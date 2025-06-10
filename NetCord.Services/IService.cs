using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace NetCord.Services;

public interface IService
{
    /// <summary>
    /// Adds all public modules defined in the specified assembly to the service.
    /// </summary>
    /// <param name="assembly">Assembly containing the modules to add.</param>
    [RequiresUnreferencedCode("Types might be removed")]
    public void AddModules(Assembly assembly);
}
