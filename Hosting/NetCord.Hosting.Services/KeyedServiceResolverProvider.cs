using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using NetCord.Services;

namespace NetCord.Hosting.Services;

#if NetCordHostingServices
public
#else
internal
#endif
class KeyedServiceResolverProvider : IServiceResolverProvider
{
    public static KeyedServiceResolverProvider Instance { get; } = new();

    private KeyedServiceResolverProvider()
    {
    }

    public Func<IServiceProvider, object?> GetResolver(ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;

        if (parameter.GetCustomAttribute<FromKeyedServicesAttribute>() is { Key: var key })
            return services => services is IKeyedServiceProvider keyedServices
                                ? keyedServices.GetKeyedService(parameterType, key)
                                : services.GetService(parameterType);

        return services => services.GetService(parameterType);
    }
}
