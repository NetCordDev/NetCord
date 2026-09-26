using System.Reflection;

namespace NetCord.Services;

public class ServiceResolverProvider : IServiceResolverProvider
{
    public static ServiceResolverProvider Instance { get; } = new();

    private ServiceResolverProvider()
    {
    }

    public Func<IServiceProvider, object?> GetResolver(ParameterInfo parameter)
    {
        var parameterType = parameter.ParameterType;
        return services => services.GetService(parameterType);
    }
}
