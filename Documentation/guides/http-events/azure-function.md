# Running Serverless C# Discord Bots on Azure Functions

Azure Functions is a great serverless hosting option for HTTP-based C# Discord bots. It lets you run your code without provisioning or managing underlying servers, which makes it a scalable and cost-effective hosting choice.

Whether you are building an interactive bot using HTTP Interactions or a background integration that listens for Webhook Events, this guide will walk you through the steps to deploy your C# Discord application to Azure Functions.

> [!NOTE]
> This guide assumes you have a basic understanding of Azure Functions. It specifically uses the Azure Functions in the isolated worker model with ASP.NET Core integration. See [Guide for running C# Azure Functions in the isolated worker model](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide) for more information.

## 1. Project Setup and Optimization

To get started, create a new project using the command below. This will set up an Azure Functions project using the isolated worker model with .NET 10.0 as the target framework.

```bash
func init --worker-runtime dotnet-isolated --target-framework net10.0
```

### Adding Required Dependencies
Add the following NuGet packages:
* [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)
* [libsodium](https://www.nuget.org/packages/libsodium)

## 2. Writing the Application

Update your `Program.cs` file to match the implementation below. Choose the section for your preferred request type.

### [Http Interactions](#tab/http-interactions)

This sets up a simple HTTP interaction bot featuring a basic `/ping` command.

[!code-cs[Program.cs](AzureFunction.HttpInteractions/Program.cs)]

Note the `REGISTER_COMMANDS` environment variable.

Because Azure Functions start and stop frequently, registering commands on every cold start wastes resources and adds latency.

Before deploying, register the commands by running your bot locally (e.g., `dotnet run`) with the `REGISTER_COMMANDS` environment variable set to `1`. (This requires the bot token - see [Configuring Secrets](#4-configuring-secrets).) You can pass that environment variable via `local.settings.json`:

[!code-json[local.settings.json](AzureFunction.HttpInteractions/local.settings.json)]

The `registerCommands` variable gates the `AutoRegisterCommands` option - it is `true` only when `REGISTER_COMMANDS` is set to `1` locally, and `false` when the app is running in Azure Functions.

### [Webhook Events](#tab/webhook-events)

This sets up an application-authorized webhook event handler, allowing you to detect when a user authorizes with your application.

[!code-cs[Program.cs](AzureFunction.WebhookEvents/Program.cs)]

> [!NOTE]
> Unlike application commands, webhook events do not require any command registration.

***

## 3. Deploying to Azure Functions

With your code ready, you can deploy the project. Use the following Azure CLI command to package your application and publish it directly to Azure Functions:

```bash
func azure functionapp publish <APP_NAME>
```

Once the deployment completes, the Azure endpoint URL will be printed in your console.

## 4. Configuring Secrets

Before your app can receive and verify requests from Discord at your new endpoint, you must provide the Public Key to your Function App. We will use environment variables for this purpose.

1. In the Azure Portal, navigate to your Function App.
2. Expand the "Settings" section and click on "Environment variables".
3. Add a new environment variable with the name `Discord__PublicKey` and set its value to your app's Public Key from the Discord Developer Portal.

If your app also makes authenticated @NetCord.Rest.RestClient calls, add a `Discord__Token` environment variable the same way.

For HTTP interactions, the bot token is also required for local command registration via `REGISTER_COMMANDS`.

> [!NOTE]
> If you want maximum security for your sensitive credentials, consider using Azure Key Vault instead. See [Use Azure Key Vault configuration provider in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-10.0) for more information.
