using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.DependenciesInjection
{
    public static class ServiceCollectionLoggingExtensions
    {
        public static IServiceCollection EnableServiceLogging(this IServiceCollection services)
        {
            var descriptors = services.ToList();

            foreach (var descriptor in descriptors)
            {
                if (!descriptor.ServiceType.IsInterface)
                    continue;

                if (descriptor.ImplementationType == null)
                    continue;

                if (descriptor.ServiceType.IsGenericTypeDefinition)
                    continue;

                if (descriptor.ImplementationType.IsGenericTypeDefinition)
                    continue;

                var assembly = descriptor.ImplementationType.Assembly.GetName().Name;

                if (assembly != "Application" &&
                    assembly != "Infrastructure")
                {
                    continue;
                }

                var serviceType = descriptor.ServiceType;
                var implementationType = descriptor.ImplementationType;
                var lifetime = descriptor.Lifetime;

                services.Remove(descriptor);

                services.Add(new ServiceDescriptor(
                    serviceType,
                    provider =>
                    {
                        var implementation =
                            ActivatorUtilities.CreateInstance(provider, implementationType);

                        var proxyType = typeof(Infrastructure.Services.LoggingDispatchProxy<>)
                            .MakeGenericType(serviceType);

                        var proxy = DispatchProxy.Create(serviceType, proxyType);

                        var loggerType = typeof(ILogger<>).MakeGenericType(serviceType);
                        var logger = provider.GetRequiredService(loggerType);

                        proxyType.GetMethod("Configure")!
                            .Invoke(proxy, new object[]
                            {
                                implementation,
                                logger
                            });

                        return proxy!;
                    },
                    lifetime));
            }

            return services;
        }
    }
}