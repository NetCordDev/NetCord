---
uid: component-interactions
---

# Introduction

## [.NET Generic Host](#tab/generic-host)

Adding component interactions with the .NET Generic Host is very easy. Use @NetCord.Hosting.Services.ComponentInteractions.ComponentInteractionServiceServiceCollectionExtensions.AddComponentInteractions``2(Microsoft.Extensions.DependencyInjection.IServiceCollection) to add a component interaction service to your host builder. Then, use @NetCord.Hosting.Services.ComponentInteractions.ComponentInteractionServiceHostExtensions.AddComponentInteraction* to add a component interaction using the minimal APIs way and/or use @NetCord.Hosting.Services.ServicesHostExtensions.AddModules(Microsoft.Extensions.Hosting.IHost,System.Reflection.Assembly) to add component interaction modules from an assembly. You also need to use @NetCord.Hosting.Gateway.GatewayEventHandlerHostExtensions.UseGatewayEventHandlers(Microsoft.Extensions.Hosting.IHost) to bind the service event handlers.

Unlike application commands, component interactions require maintaining context for each interaction, as the data between component interaction types can vary significantly. While it's possible to use a single context for all component interactions and cast them as needed, it's generally recommended to use distinct contexts for different types of component interactions. This is why multiple services are added to the service collection here - each one handles a specific type of component interaction. However, you likely won't need all of them, so feel free to remove the ones that aren't relevant to your use case.
[!code-cs[Program.cs](IntroductionHosting/Program.cs?highlight=13-19,24-30,33,36)]

## [Bare Bones](#tab/bare-bones)

First, add the following lines to the using section.
[!code-cs[Program.cs](Introduction/Program.cs#L3-L5)]

Now, it's time to create @NetCord.Services.ComponentInteractions.ComponentInteractionService`1 instance and add component interactions to it. In this example, we will use @NetCord.Services.ComponentInteractions.ButtonInteractionContext.
[!code-cs[Program.cs](Introduction/Program.cs#L12-L19)]

We can add an interaction handler now.
[!code-cs[Program.cs](Introduction/Program.cs#L21-L43)]

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
