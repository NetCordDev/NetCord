using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;

namespace GettingStarted;

/// <summary>
/// Examples for the Running Your Bot guide.
/// </summary>
public static class RunningYourBot
{
    // TODO: Add bot execution examples
    // - Starting the host
    // - Graceful shutdown
    // - Handling cancellation
    
    public static async Task RunBasicBotAsync(CancellationToken cancellationToken = default)
    {
        var builder = Host.CreateApplicationBuilder();
        
        builder.Services.AddDiscordGateway();
        
        var host = builder.Build();
        
        await host.RunAsync(cancellationToken);
    }
}
