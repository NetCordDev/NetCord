using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetCord.Services;

namespace NetCord.Hosting.Services;

public static class ServicesHostExtensions
{
    [RequiresUnreferencedCode("Types might be removed")]
    public static IHost AddModules(this IHost host, Assembly assembly)
    {
        var services = host.Services.GetServices<IService>();
        foreach (var service in services)
            service.AddModules(assembly);

        return host;
    }
}
