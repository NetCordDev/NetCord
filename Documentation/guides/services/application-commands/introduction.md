---
uid: application-commands
---

# Introduction

## [.NET Generic Host](#tab/generic-host)

Adding application commands with the .NET Generic Host is very easy. Use @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceServiceCollectionExtensions.AddApplicationCommands``2(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add the application command service to your host builder. Then, use @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddSlashCommand*, @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddUserCommand* or @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddMessageCommand* to add an application command using the minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add application command modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=14,19-21,24,27)]

## [Bare Bones](#tab/bare-bones)

First, add the following lines to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L3-L5)]

Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add application commands to it. You can do it by using @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddSlashCommand*, @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddUserCommand* or @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddMessageCommand* to add an application command using the minimal APIs way and/or by using @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddModules(System.Reflection.Assembly) to add application command modules from an assembly.
[!code-cs[Program.cs](Introduction/Program.cs#L12-L21)]

We can add a command handler now. If you used a context other than @NetCord.Services.ApplicationCommands.ApplicationCommandContext, you may need to change the interaction type of the handler to the appropriate one.
[!code-cs[Program.cs](Introduction/Program.cs#L23-L45)]

Now, we should send the commands to Discord, to make them usable. Add the following line under the handler:
[!code-cs[Program.cs](Introduction/Program.cs#L47-L48)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

> [!NOTE]
> If you don't see the commands in Discord, try refreshing the Discord client using `Ctrl + R` on PC or `⌘ + R` on Mac.

> [!IMPORTANT]
> Please note that names of slash commands must be lowercase.

### Example Module

Here you can see an example module showing how to use modules with application commands.
[!code-cs[ExampleModule.cs](Introduction/ExampleModule.cs)]
