using NetCord.Hosting.Rest;
using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Services.ApplicationCommands;

using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.APIGatewayEvents;

using System.Text.Json.Serialization;

var registerCommands = args.Contains("--register-commands");

var builder = WebApplication.CreateSlimBuilder(args);

var services = builder.Services;

services
    .AddDiscordRest()
    .AddHttpApplicationCommands(o => o.AutoRegisterCommands = registerCommands)
    .AddAWSLambdaHosting(LambdaEventSource.HttpApi,
                         // That is only required when using Native AOT,
                         // otherwise that parameter can be omitted
                         new SourceGeneratorLambdaJsonSerializer<APIGatewaySerializerContext>());

var app = builder.Build();

app.AddSlashCommand("ping", "Ping AWS Lambda", () => "Pong from AWS Lambda!");

if (registerCommands)
{
    await app.StartAsync();
    await app.StopAsync();
    return;
}

app.UseHttpInteractions("/");

await app.RunAsync();

// That is passed to AddAWSLambdaHosting for Native AOT,
// otherwise that class can be omitted
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
public partial class APIGatewaySerializerContext : JsonSerializerContext;
