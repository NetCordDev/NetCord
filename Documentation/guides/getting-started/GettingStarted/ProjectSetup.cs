using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;

namespace GettingStarted;

/// <summary>
/// Examples for the Project Setup guide.
/// </summary>
public static class ProjectSetup
{
    // TODO: Add project configuration examples
    // - Generic Host configuration
    // - appsettings.json structure
    // - Environment variables
    // - Secrets management
    
    public static void ConfigureServices()
    {
        var builder = Host.CreateApplicationBuilder();
        
        // Example: Adding Discord Gateway
        builder.Services.AddDiscordGateway();
        
        // TODO: Add more configuration examples
    }
}
