using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Hosting.Services.ApplicationCommands;

var registerCommands = Environment.GetEnvironmentVariable("REGISTER_COMMANDS") is "1";

var builder = FunctionsApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.ConfigureFunctionsWebApplication();

var services = builder.Services;

services.AddDiscordRest()
        .AddHttpApplicationCommands(o => o.AutoRegisterCommands = registerCommands)
        .AddHttpInteractionProcessor();

var host = builder.Build();

host.AddSlashCommand("ping", "Ping!", () => "Pong from Azure Function!");

await host.RunAsync();

public class Interaction(IHttpInteractionProcessor processor)
{
    [Function("interaction")]
    public Task RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        return processor.ProcessAsync(request.HttpContext);
    }
}
