# Introduction

## [Hosting](#tab/hosting)

With hosting, adding commands is very easy. Use @NetCord.Hosting.Services.Commands.CommandServiceHostBuilderExtensions.UseCommandService``1(Microsoft.Extensions.Hosting.IHostBuilder) to add a command service to your host builder. Then, use @NetCord.Hosting.Services.Commands.CommandServiceHostExtensions.AddCommand* to add a command using the ASP.NET Core minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=10,14-16)]

### Specifying a prefix

You can specify a prefix in the configuration. You can for example use `appsettings.json` file. It should look like this:
[!code-json[appsettings.json](IntroductionHosting/appsettings.json)]

## [Without Hosting](#tab/without-hosting)

First, add the following line to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L3)]

Now, it's time to create @NetCord.Services.Commands.CommandService`1 instance and add modules to it.
[!code-cs[Program.cs](Introduction/Program.cs#L10-L11)]

We can add a command handler now.
[!code-cs[Program.cs](Introduction/Program.cs#L13-L32)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

### Example Module

[!code-cs[ExampleModule.cs](Introduction/ExampleModule.cs)]