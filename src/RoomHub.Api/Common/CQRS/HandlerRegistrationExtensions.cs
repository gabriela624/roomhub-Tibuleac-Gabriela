using System.Reflection;

namespace RoomHub.Api.Common.CQRS;

public static class HandlerRegistrationExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var type in types)
        {
            var interfaces = type.GetInterfaces()
                .Where(interfaceType =>
                    interfaceType.IsGenericType &&
                    (interfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     interfaceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

            foreach (var interfaceType in interfaces)
                services.AddScoped(interfaceType, type);
        }

        return services;
    }
}