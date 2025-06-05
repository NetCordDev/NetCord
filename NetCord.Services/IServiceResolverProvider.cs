using System.Reflection;

namespace NetCord.Services;

#if NetCord_Services
public
#else
internal
#endif
interface IServiceResolverProvider
{
    public Func<IServiceProvider, object?> GetResolver(ParameterInfo parameter);
}
