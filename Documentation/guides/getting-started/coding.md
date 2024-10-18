# Setting Up Your First C# Discord Bot with NetCord

> [!NOTE]
> This guide assumes you have already created a project with NetCord installed and a bot set up in the [Discord Developer Portal](https://discord.com/developers/applications). If you haven't, please return to the previous guides for setup instructions.

This guide will walk you through setting up your first C# Discord bot using NetCord. It will show you how to create a simple bot that connects to Discord. Nothing too fancy, just the basics to get you started. Let's dive in!

## [Generic Host](#tab/generic-host)

> [!NOTE]
> The generic host way requires the following packages:
> - [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting)
> - [NetCord.Hosting](https://www.nuget.org/packages/NetCord.Hosting)

With the generic host, you can just use @NetCord.Hosting.Gateway.GatewayClientServiceCollectionExtensions.AddDiscordGateway(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add your bot to the host. Quite easy, right?
[!code-cs[Program.cs](CodingHosting/Program.cs)]

Also note that the token should be stored in the configuration. You can for example use `appsettings.json` file. It should look like this:
[!code-json[appsettings.json](CodingHosting/appsettings.json)]

## [Bare Bones](#tab/bare-bones)

Add the following lines to the `Program.cs` file to create the @NetCord.Gateway.GatewayClient.
[!code-cs[Program.cs](Coding/Program.cs#L1-L4)]

Then add logging, to know what your bot is doing.
[!code-cs[Program.cs](Coding/Program.cs#L6-L10)]

Now it's time to finally... make the bot online! To do it, add the following lines to your code.
[!code-cs[Program.cs](Coding/Program.cs#L12-L13)]

### The Final Product
[!code-cs[Program.cs](Coding/Program.cs)]

***

Now, when you run the code, your bot should be online!
![Bot being online](../../images/coding_BotOnline.png)
