using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.DependenciesInjection
{
    public static class ServiceCollectionLoggingExtensions
    {
        /// <summary>
        /// Wraps interface-based service registrations with a DispatchProxy that logs method entry/exit/exceptions.
        /// Call this after the normal service registrations are added.
        /// </summary>
        public static IServiceCollection EnableServiceLogging(this IServiceCollection services)
        {
            // Capture descriptors to avoid modifying collection while enumerating
            var descriptors = services.ToList();

            foreach (var descriptor in descriptors)
            {
                // Only wrap service registrations that are interface -> concrete
                // and not open-generic registrations. Wrapping open-generic
                // registrations (e.g. IOptions<>) with an implementation factory
                // causes the DI container to throw because open-generic service
                // types require open-generic implementation types.
                if (!descriptor.ServiceType.IsInterface || descriptor.ImplementationType == null)
                    continue;

                if (descriptor.ServiceType.IsGenericTypeDefinition)
                    continue;

                if (descriptor.ImplementationType.IsGenericTypeDefinition)
                    continue;

                // now safe to wrap
                    var serviceType = descriptor.ServiceType;
                    var implType = descriptor.ImplementationType;

                    // Replace the registration with a factory that builds the implementation and wraps it with proxy
                    services.Remove(descriptor);

                    services.Add(new ServiceDescriptor(serviceType, provider =>
                    {
                        // create the actual implementation
                        var impl = ActivatorUtilities.CreateInstance(provider, implType);

                        // create logger for the service interface
                        var logger = provider.GetService(typeof(ILogger<>).MakeGenericType(serviceType)) as ILogger;

                        // create proxy
                        var proxyType = typeof(Infrastructure.Services.LoggingDispatchProxy<>).MakeGenericType(serviceType);
                        var proxy = DispatchProxy.Create(serviceType, proxyType);

                        // configure proxy: call Configure(T decorated, ILogger<T> logger)
                        var configureMethod = proxyType.GetMethod("Configure");
                        if (configureMethod != null)
                        {
                            // logger may be null if not registered; create a generic logger fallback
                            var loggerType = typeof(ILogger<>).MakeGenericType(serviceType);
                            var loggerInstance = provider.GetService(loggerType);

                            configureMethod.Invoke(proxy, new[] { impl, loggerInstance });
                        }

                        return proxy;
                    }, descriptor.Lifetime));
            }

            return services;
        }
    }
}
