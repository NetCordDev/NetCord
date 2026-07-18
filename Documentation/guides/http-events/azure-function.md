# Running Serverless C# Discord Bots on Azure Functions

Deploying HTTP-based C# Discord bots on Azure Functions is an excellent way to leverage a serverless architecture. Azure Functions allows you to run your code without provisioning or managing underlying servers, making it a highly scalable and cost-effective hosting choice for your bot.

This guide will walk you through the steps to deploy your C# Discord bot to Azure Functions.

> [!NOTE]
> This guide assumes you have a basic understanding of Azure Functions. It specifically uses the Azure Functions in the isolated worker model with ASP.NET Core integration. See [Guide for running C# Azure Functions in the isolated worker model](https://learn.microsoft.com/azure/azure-functions/dotnet-isolated-process-guide) for more information.

## 1. Project Setup and Optimization

To get started, create a new project using the command below. This will set up an Azure Functions project using the isolated worker model with .NET 10.0 as the target framework.

```bash
func init --worker-runtime dotnet-isolated --target-framework net10.0
```

### Adding Required Dependencies
Next, add the necessary NuGet packages to power the bot and handle cryptographic operations:
* [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)
* [libsodium](https://www.nuget.org/packages/libsodium)

## 2. Writing the Bot Application

Now it's time to write the code. Update your `Program.cs` file to match the implementation below. This sets up a simple HTTP interaction bot featuring a basic `/ping` command.

[!code-cs[Program.cs](AzureFunction/Program.cs)]

Notice the inclusion of the `REGISTER_COMMANDS` environment variable. In a serverless environment like Azure Functions, your application starts and stops frequently. Registering commands on every boot wastes resources and slows down startup times.

When deploying, run your bot locally (e.g., `dotnet run`) with the `REGISTER_COMMADNS` environment variable set to `1` to register the commands. You can pass that environment variable via `local.settings.json`:

[!code-json[local.settings.json](AzureFunction/local.settings.json)]

Note that registering commands requires the bot token, see [Configuring Secrets](#4-configuring-secrets).

You can also use the `registerCommands` variable to load certain services specifically when the bot is running in Azure Functions.

## 3. Deploying to Azure Functions

With your code ready, you can deploy the bot. Use the following Azure CLI command to package your application and publish it directly to Azure Functions:

```bash
func azure functionapp publish <APP_NAME>
```

Once the deployment completes, the Azure endpoint URL will be printed in your console.

## 4. Configuring Secrets

Before your bot can receive and verify interactions from Discord at your new endpoint, you must add your bot's Public Key to the Azure Function. We will use environment variables for this purpose.

1. In the Azure Portal, navigate to your Function App.
2. Expand the "Settings" section and click on "Environment variables".
3. Add a new environment variable with the name `Discord__PublicKey` and set its value to your bot's Public Key from the Discord Developer Portal.

If you are building a more complex bot that requires authenticated @NetCord.Rest.RestClient usage, you will also need to provide your bot token. To do so, add an environment variable named `Discord__Token` in the same way.

The bot token is also required to register commands locally using the `REGISTER_COMMANDS` environment variable, so ensure it gets provided.

> [!NOTE]
> If you want maximum security for your sensitive credentials, consider using Azure Key Vault instead. See [Use Azure Key Vault configuration provider in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/key-vault-configuration?view=aspnetcore-10.0) for more information.
