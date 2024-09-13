# Introduction

> [!IMPORTANT]
> Please note that names of:
> - slash commands
> - sub slash commands
> - slash command parameters
> 
> **must** be lowercase.

## [Generic Host](#tab/generic-host)

Adding application commands with the generic host is very easy. Use @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceServiceCollectionExtensions.AddApplicationCommands``2(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add an application command service to your host builder. Then, use @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddSlashCommand*, @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddUserCommand* or @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceHostExtensions.AddMessageCommand* to add an application command using the ASP.NET Core minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=13-15,18-22)]

## [Bare Bones](#tab/bare-bones)

First, add the following lines to using the section.
[!code-cs[Program.cs](Introduction/Program.cs#L4-L5)]

Now, it's time to create @NetCord.Services.ApplicationCommands.ApplicationCommandService`1 instance and add application commands to it. You can do it by using @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddSlashCommand*, @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddUserCommand* or @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddMessageCommand* to add an application command using the ASP.NET Core minimal APIs way and/or by using @NetCord.Services.ApplicationCommands.ApplicationCommandService`1.AddModules(System.Reflection.Assembly) to add modules from an assembly. You can use a context of your choice, it can be for example @NetCord.Services.ApplicationCommands.SlashCommandContext, @NetCord.Services.ApplicationCommands.UserCommandContext or @NetCord.Services.ApplicationCommands.MessageCommandContext. In this example, we will use @NetCord.Services.ApplicationCommands.SlashCommandContext.
[!code-cs[Program.cs](Introduction/Program.cs#L12-L14)]

We can add a command handler now. If you used other context than @NetCord.Services.ApplicationCommands.SlashCommandContext, you should change the interaction type of the handler to the appropriate one.
[!code-cs[Program.cs](Introduction/Program.cs#L16-L33)]

Now, we should send the commands to Discord, to make them usable. Add the following line under the handler:
[!code-cs[Program.cs](Introduction/Program.cs#L35)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

### Example Modules

Here you can see example modules for each type of application command.

#### Slash Command Module
[!code-cs[SlashCommandModule.cs](Introduction/SlashCommandModule.cs)]

#### User Command Module
[!code-cs[UserCommandModule.cs](Introduction/UserCommandModule.cs)]

#### Message Command Module
[!code-cs[MessageCommandModule.cs](Introduction/MessageCommandModule.cs)]