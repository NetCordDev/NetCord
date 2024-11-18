# Introduction

## [.NET Generic Host](#tab/generic-host)

Adding commands with the .NET Generic Host is very easy. Use @NetCord.Hosting.Services.Commands.CommandServiceServiceCollectionExtensions.AddCommands``1(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add the command service to your host builder. Then, use @NetCord.Hosting.Services.Commands.CommandServiceHostExtensions.AddCommand* to add a command using the minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add command modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=12,17,20,23)]

### Specifying a prefix

You can specify a prefix in the configuration. You can for example use `appsettings.json` file. It should look like this:
[!code-json[appsettings.json](IntroductionHosting/appsettings.json)]

## [Bare Bones](#tab/bare-bones)

First, add the following lines to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L3-L4)]

Now, it's time to create @NetCord.Services.Commands.CommandService`1 instance and add commands to it.
[!code-cs[Program.cs](Introduction/Program.cs#L11-L18)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/Program.cs#L20-L42)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

### Example Module

[!code-cs[ExampleModule.cs](Introduction/ExampleModule.cs)]
