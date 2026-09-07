# Running Serverless C# Discord Bots on AWS Lambda

Deploying HTTP-based C# Discord bots and applications on AWS Lambda is an excellent way to leverage a serverless architecture. AWS Lambda allows you to run your code without provisioning or managing underlying servers, making it a highly scalable and cost-effective hosting choice for your project.

Whether you are building an interactive bot using HTTP Interactions or a background integration listening for Webhook Events, this guide will walk you through the steps to deploy your C# Discord application to AWS Lambda. It will also cover how to enable Native AOT to drastically reduce your cold start times.

> [!NOTE]
> This guide assumes you have a basic understanding of AWS Lambda. It specifically uses the AWS Lambda ASP.NET Core integration for seamless setup. See [Deploy ASP.NET applications](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-asp.html) for more information.

## 1. Project Setup and Optimization

To get started, create a new project using the `serverless.AspNetCoreMinimalAPI` template. If you aren't familiar with this template, refer to the AWS documentation linked in the note above.

Once generated, we need to clean up and optimize the configuration.

### Upgrading to the HTTP API
By default, the template configures a REST API. We can optimize costs and performance by switching to the newer HTTP API. Make the following changes to your `serverless.template` file:

[!code-diff[serverless.template](AWSLambda.HttpInteractions/serverless.template.diff)]

### Removing Unnecessary Files
Remove the `Controllers` directory with its contents as they are not needed.

### Enabling Native AOT
To reduce your app's cold start times in a serverless environment, we highly recommend enabling Native AOT compilation. Update your project file as follows:

[!code-diff[AWSLambda.csproj](AWSLambda.HttpInteractions/AWSLambda.HttpInteractions.csproj.diff)]

### Adding Required Dependencies
Next, add the necessary NuGet packages to power the app and handle cryptographic operations:
* [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)
* [libsodium](https://www.nuget.org/packages/libsodium)

## 2. Writing the Application

Now it's time to write the code. Update your `Program.cs` file to match the implementation below. Choose the appropriate tab for your preferred request type.

### [Http Interactions](#tab/http-interactions)

This sets up a simple HTTP interaction bot featuring a basic `/ping` command.

[!code-cs[Program.cs](AWSLambda.HttpInteractions/Program.cs)]

Notice the inclusion of the `--register-commands` flag. In a serverless environment like AWS Lambda, your application starts and stops frequently. Registering commands on every boot wastes resources and slows down startup times. 

When deploying, run your bot locally to register the commands (e.g., `dotnet run -- --register-commands`). Note that registering commands requires the bot token, see [Configuring Secrets](#4-configuring-secrets).

You can also use the `registerCommands` variable to load certain services specifically when the bot is running in AWS Lambda.

### [Webhook Events](#tab/webhook-events)

This sets up an application authorized webhook event handler, allowing you to detect when a user authorizes with your application.

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

Before your app can receive and verify requests from Discord at your new endpoint, you must add a Public Key to the Lambda. We will use environment variables for this purpose.

1. In the AWS Management Console, navigate to your Lambda function.
2. Click on **Configuration**, then select **Environment variables**.
3. Add a new environment variable with the key `Discord__PublicKey` and set its value to your app's Public Key from the Discord Developer Portal.

If you are building a more complex app that requires authenticated @NetCord.Rest.RestClient usage, you will also need to provide your bot token. To do so, add an environment variable named `Discord__Token` in the same way.

For HTTP interactions, the bot token is also required to register commands locally using the `--register-commands` flag, so ensure it gets provided.

> [!NOTE]
> If you want maximum security for your sensitive credentials, consider using AWS Secrets Manager instead. See [AWS .NET Configuration Extension for Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration) for more information.
