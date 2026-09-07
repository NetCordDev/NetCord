using NetCord.Hosting.Rest;
using NetCord.Hosting.AspNetCore;

using Amazon.Lambda.Serialization.SystemTextJson;
using Amazon.Lambda.APIGatewayEvents;

using System.Text.Json.Serialization;
using NetCord.Rest;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services
    .AddDiscordRest()
    .AddWebhookHandler(WebhookEvent.ApplicationAuthorized, (ApplicationAuthorizedWebhookEventArgs args,
                                                            ILogger<Program> logger) =>
    {
        logger.LogInformation("User {UserId} authorized the application with scopes: {Scopes}",
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
