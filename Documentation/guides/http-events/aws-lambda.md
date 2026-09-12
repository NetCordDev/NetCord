# Running Serverless C# Discord Bots on AWS Lambda

AWS Lambda is a great serverless hosting option for HTTP-based C# Discord bots. It lets you run your code without provisioning or managing underlying servers, which makes it a scalable and cost-effective hosting choice.

Whether you are building an interactive bot using HTTP Interactions or a background integration that listens for Webhook Events, this guide will walk you through the steps to deploy your C# Discord application to AWS Lambda. It will also cover how to enable Native AOT to drastically reduce your cold start times.

> [!NOTE]
> This guide assumes you have a basic understanding of AWS Lambda. It specifically uses the AWS Lambda ASP.NET Core integration for seamless setup. See [Deploy ASP.NET applications](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-asp.html) for more information.

## 1. Project Setup and Optimization

To get started, create a new project using the `serverless.AspNetCoreMinimalAPI` template. If you aren't familiar with this template, refer to the AWS documentation linked in the note above.

Once generated, clean up and optimize the configuration.

### Upgrading to the HTTP API
By default, the template configures a REST API. You can optimize costs and performance by switching to the newer HTTP API. Make the following changes to your `serverless.template` file:

[!code-diff[serverless.template](AWSLambda.HttpInteractions/serverless.template.diff)]

### Removing Unnecessary Files
Remove the `Controllers` directory and its contents - they are not needed for this setup.

### Enabling Native AOT
To reduce your app's cold start times in a serverless environment, we highly recommend enabling Native AOT compilation. Update your project file as follows:

[!code-diff[AWSLambda.csproj](AWSLambda.HttpInteractions/AWSLambda.HttpInteractions.csproj.diff)]

### Adding Required Dependencies
Add the following NuGet packages:
* [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)
* [libsodium](https://www.nuget.org/packages/libsodium)

## 2. Writing the Application

Update your `Program.cs` file to match the implementation below. Choose the section for your preferred request type.

### [Http Interactions](#tab/http-interactions)

This sets up a simple HTTP interaction bot featuring a basic `/ping` command.

[!code-cs[Program.cs](AWSLambda.HttpInteractions/Program.cs)]

Note the `--register-commands` flag. Because Lambda functions start and stop frequently, registering commands on every cold start wastes resources and adds latency.

Before deploying, register the commands by running your bot locally with the flag: `dotnet run -- --register-commands`. (This requires the bot token - see [Configuring Secrets](#4-configuring-secrets).)

The `registerCommands` variable gates the `AutoRegisterCommands` option - it is `true` only when you explicitly pass `--register-commands` locally, and `false` when the app is running in Lambda.

### [Webhook Events](#tab/webhook-events)

This sets up an application-authorized webhook event handler, allowing you to detect when a user authorizes with your application.

[!code-cs[Program.cs](AWSLambda.WebhookEvents/Program.cs)]

> [!NOTE]
> Unlike application commands, webhook events do not require any command registration.

***

## 3. Deploying to AWS Lambda

With your code ready, you can deploy the project. Use the following .NET CLI command to package your application and publish it directly to AWS Lambda:

```bash
dotnet lambda deploy-serverless
```

Once the deployment completes, the AWS endpoint URL will be printed in your console.

## 4. Configuring Secrets

Before your app can receive and verify requests from Discord at your new endpoint, you must provide the Public Key to your Lambda function. We will use environment variables for this purpose.

1. In the AWS Management Console, navigate to your Lambda function.
2. Click on **Configuration**, then select **Environment variables**.
3. Add a new environment variable with the key `Discord__PublicKey` and set its value to your app's Public Key from the Discord Developer Portal.

If your app also makes authenticated @NetCord.Rest.RestClient calls, add a `Discord__Token` environment variable the same way.

For HTTP interactions, the bot token is also required for local command registration via `--register-commands`.

> [!NOTE]
> If you want maximum security for your sensitive credentials, consider using AWS Secrets Manager instead. See [AWS .NET Configuration Extension for Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration) for more information.
