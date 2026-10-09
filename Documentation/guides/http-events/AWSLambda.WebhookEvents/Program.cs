using NetCord.Hosting.AspNetCore;
using NetCord.Hosting.Rest;
using NetCord.Rest;

using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.APIGatewayEvents;

using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddDiscordRest()
    .AddWebhookHandler(WebhookEvent.ApplicationAuthorized, (ApplicationAuthorizedWebhookEventArgs args,
                                                            ILogger<Program> logger) =>
    {
        logger.LogInformation("User '{Username}' authorized with scopes: {Scopes}",
                              args.User.Username,
                              args.Scopes);
    })
    .AddAWSLambdaHosting(LambdaEventSource.HttpApi,
                         // That is only required when using Native AOT,
                         // otherwise that parameter can be omitted
                         new SourceGeneratorLambdaJsonSerializer<APIGatewaySerializerContext>());

var app = builder.Build();

app.UseWebhookEvents("/");

await app.RunAsync();

// That is passed to AddAWSLambdaHosting for Native AOT,
// otherwise that class can be omitted
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyRequest))]
[JsonSerializable(typeof(APIGatewayHttpApiV2ProxyResponse))]
public partial class APIGatewaySerializerContext : JsonSerializerContext;
