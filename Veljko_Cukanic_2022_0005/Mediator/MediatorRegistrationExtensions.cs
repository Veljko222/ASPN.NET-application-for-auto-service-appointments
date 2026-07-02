using AutoService.Application.Mediator;
using System.Reflection;

namespace AutoService.Web.Mediator
{
    public static class MediatorRegistrationExtensions
    {
        public static IServiceCollection AddMediatorHandlers(
            this IServiceCollection services,
            params Assembly[] assemblies)
        {
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t is { IsClass: true, IsAbstract: false });

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType
                    .GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        (i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)));

                foreach (var handlerInterface in interfaces)
                {
                    services.AddScoped(handlerInterface, handlerType);
                }
            }

            return services;
        }
    }
}

