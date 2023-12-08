using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services;

public interface IService
{
    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly);
}
