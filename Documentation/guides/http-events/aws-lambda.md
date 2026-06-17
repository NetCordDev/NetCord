# Running Serverless C# Discord Bots on AWS Lambda

Deploying HTTP-based C# Discord bots on AWS Lambda is an excellent way to leverage a serverless architecture. AWS Lambda allows you to run your code without provisioning or managing underlying servers, making it a highly scalable and cost-effective hosting choice for your bot.

This guide will walk you through the steps to deploy your C# Discord bot to AWS Lambda. It will also cover how to enable Native AOT to drastically reduce your bot's cold start times.

> [!NOTE]
> This guide assumes you have a basic understanding of AWS Lambda. It specifically uses the AWS Lambda ASP.NET Core integration for seamless setup. See [Deploy ASP.NET applications](https://docs.aws.amazon.com/lambda/latest/dg/csharp-package-asp.html) for more information.

## 1. Project Setup and Optimization

To get started, create a new project using the `serverless.AspNetCoreMinimalAPI` template. If you aren't familiar with this template, refer to the AWS documentation linked in the note above.

Once generated, we need to clean up and optimize the configuration.

### Upgrading to the HTTP API
By default, the template configures a REST API. We can optimize costs and performance by switching to the newer HTTP API. Make the following changes to your `serverless.template` file:

[!code-diff[serverless.template](AWSLambda/serverless.template.diff)]

### Removing Unnecessary Files
Remove the `Controllers` directory with its contents as they are not needed.

### Enabling Native AOT
To reduce your bot's cold start times in a serverless environment, we highly recommend enabling Native AOT compilation. Update your project file as follows:

[!code-diff[AWSLambda.csproj](AWSLambda/AWSLambda.csproj.diff)]

### Adding Required Dependencies
Next, add the necessary NuGet packages to power the bot and handle cryptographic operations:
* [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore)
* [libsodium](https://www.nuget.org/packages/libsodium)

## 2. Writing the Bot Application

Now it's time to write the code. Update your `Program.cs` file to match the implementation below. This sets up a simple HTTP interaction bot featuring a basic `/ping` command.

[!code-cs[Program.cs](AWSLambda/Program.cs)]

Notice the inclusion of the `--register-commands` flag. In a serverless environment like AWS Lambda, your application starts and stops frequently. Registering commands on every boot wastes resources and slows down startup times. 

When deploying, run your bot locally to register the commands (e.g., `dotnet run -- --register-commands`). Note that registering commands requires the bot token, see [Configuring Secrets](#4-configuring-secrets).

You can also use the `registerCommands` variable to load certain services specifically when the bot is running in AWS Lambda.

## 3. Deploying to AWS Lambda

With your code ready, you can deploy the bot. Use the following .NET CLI command to package your application and publish it directly to AWS Lambda:

```bash
dotnet lambda deploy-serverless
```

Once the deployment completes, the AWS endpoint URL will be printed in your console.

## 4. Configuring Secrets

Before your bot can receive and verify interactions from Discord at your new endpoint, you must add your bot's Public Key to the Lambda function. We will use environment variables for this purpose.

1. In the AWS Management Console, navigate to your Lambda function.
2. Click on **Configuration**, then select **Environment variables**.
3. Add a new variable named `Discord__PublicKey` (note the double underscore).
4. Paste your bot's Public Key, which can be found in the Discord Developer Portal.

If you are building a more complex bot that requires authenticated @NetCord.Rest.RestClient usage, you will also need to provide your bot token. To do so, add an environment variable named `Discord__Token` in the same way.

The bot token is also required to register commands locally using the `--register-commands` flag, so ensure it gets provided.

> [!NOTE]
> If you want maximum security for your sensitive credentials, consider using AWS Secrets Manager instead. See [AWS .NET Configuration Extension for Systems Manager](https://github.com/aws/aws-dotnet-extensions-configuration) for more information.
