using System.Reflection;

namespace NetCord.Services;

#if NetCordServices
public
#else
internal
#endif
interface IServiceResolverProvider
{
    public Func<IServiceProvider, object?> GetResolver(ParameterInfo parameter);
}
