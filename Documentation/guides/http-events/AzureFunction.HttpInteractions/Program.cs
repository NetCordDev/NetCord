using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

var registerCommands = Environment.GetEnvironmentVariable("REGISTER_COMMANDS") is "1";

var builder = FunctionsApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddDiscordRest()
    .AddHttpApplicationCommands(o => o.AutoRegisterCommands = registerCommands)
    .AddHttpInteractionProcessor();

var host = builder.Build();

host.AddSlashCommand("ping", "Ping!", () => "Pong from Azure Function!");

await host.RunAsync();

public class Interaction(IHttpInteractionProcessor processor)
{
    [Function("interaction")]
    public ValueTask RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        return processor.ProcessAsync(request.HttpContext);
    }
}
