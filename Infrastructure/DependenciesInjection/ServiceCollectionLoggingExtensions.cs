using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Infrastructure.DependenciesInjection
{
    public static class ServiceCollectionLoggingExtensions
    {
        /// <summary>
        /// Wraps application interface-based services with LoggingDispatchProxy.
        /// 
        /// This method should be called after registering
        /// Infrastructure and Application services.
        /// </summary>
        public static IServiceCollection EnableServiceLogging(
            this IServiceCollection services)
        {
            // Create a copy because we will modify the original collection
            var descriptors = services.ToList();

            foreach (var descriptor in descriptors)
            {
                // Only interface registrations
                if (!descriptor.ServiceType.IsInterface)
                {
                    continue;
                }

                // Only registrations with concrete implementation types
                if (descriptor.ImplementationType == null)
                {
                    continue;
                }

                // Skip open generic registrations
                if (descriptor.ServiceType.IsGenericTypeDefinition)
                {
                    continue;
                }

                if (descriptor.ImplementationType.IsGenericTypeDefinition)
                {
                    continue;
                }

                // Only wrap services belonging to the Application layer
                if (descriptor.ServiceType.Namespace == null ||
                    !descriptor.ServiceType.Namespace
                        .StartsWith("Application"))
                {
                    continue;
                }

                var serviceType =
                    descriptor.ServiceType;

                var implementationType =
                    descriptor.ImplementationType;

                var lifetime =
                    descriptor.Lifetime;

                // Remove original registration
                services.Remove(descriptor);

                // Add proxied registration
                services.Add(
                    new ServiceDescriptor(
                        serviceType,
                        provider =>
                        {
                            // Create actual implementation
                            var implementation =
                                ActivatorUtilities.CreateInstance(
                                    provider,
                                    implementationType);

                            // Create LoggingDispatchProxy<T>
                            var proxyType =
                                typeof(LoggingDispatchProxy<>)
                                    .MakeGenericType(serviceType);

                            var proxy =
                                DispatchProxy.Create(
                                    serviceType,
                                    proxyType);

                            // Resolve ILogger<ImplementationType>
                            var loggerType =
                                typeof(ILogger<>)
                                    .MakeGenericType(
                                        implementationType);

                            var logger =
                                provider.GetService(
                                    loggerType);

                            // Configure proxy
                            var configureMethod =
                                proxyType.GetMethod(
                                    "Configure");

                            if (configureMethod == null)
                            {
                                throw new InvalidOperationException(
                                    $"Configure method was not found on {proxyType.Name}.");
                            }

                            configureMethod.Invoke(
                                proxy,
                                new object?[]
                                {
                                    implementation,
                                    logger
                                });

                            return proxy;
                        },
                        lifetime));
            }

            return services;
        }
    }
}