# Introduction

## [Generic Host](#tab/generic-host)

Adding component interactions with the generic host is very easy. Use @NetCord.Hosting.Services.ComponentInteractions.ComponentInteractionServiceServiceCollectionExtensions.AddComponentInteractions``2(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add a component interaction service to your host builder. Then, use @NetCord.Hosting.Services.ComponentInteractions.ComponentInteractionServiceHostExtensions.AddComponentInteraction* to add a component interaction using the ASP.NET Core minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=13-19,22-30)]

## [Bare Bones](#tab/bare-bones)

First, add the following lines to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L4-L5)]

Now, it's time to create @NetCord.Services.ComponentInteractions.ComponentInteractionService`1 instance and add modules to it. In this example, we will use @NetCord.Services.ComponentInteractions.ButtonInteractionContext.
[!code-cs[Program.cs](Introduction/Program.cs#L12-L13)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/Program.cs#L15-L32)]

### The Final Product

#### Program.cs
[!code-cs[Program.cs](Introduction/Program.cs)]

***

### Example Modules

#### Button Module
[!code-cs[ButtonModule.cs](Introduction/ButtonModule.cs)]

#### String Menu Module
[!code-cs[StringMenuModule.cs](Introduction/StringMenuModule.cs)]

#### User Menu Module
[!code-cs[UserMenuModule.cs](Introduction/UserMenuModule.cs)]

#### Role Menu Module
[!code-cs[RoleMenuModule.cs](Introduction/RoleMenuModule.cs)]

#### Mentionable Menu Module
[!code-cs[MentionableMenuModule.cs](Introduction/MentionableMenuModule.cs)]

#### Channel Menu Module
[!code-cs[ChannelMenuModule.cs](Introduction/ChannelMenuModule.cs)]

#### Modal Module
[!code-cs[ModalModule.cs](Introduction/ModalModule.cs)]
