---
omitAppTitle: true
title: Handling Discord HTTP Interactions and Webhook Events with C# and ASP.NET Core
description: Learn how to handle Discord HTTP Interactions and Webhook Events using the NetCord.Hosting.AspNetCore package in C# and ASP.NET Core.
---

# Handling Discord HTTP Events with ASP.NET Core

This guide will show you how to receive and handle Discord events through HTTP requests using the [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore) package. This package can also be effortlessly integrated with [NetCord.Hosting.Services](https://www.nuget.org/packages/NetCord.Hosting.Services) to handle HTTP interactions in C# easily.

## Required Dependencies

Before you get started, ensure that you've installed the necessary native dependencies. Follow the [installation guide](../installing-native-dependencies.md) to set them up.

## Setting Up

To receive Discord events from Discord in your bot, you need to use @NetCord.Hosting.Rest.RestClientServiceCollectionExtensions.AddDiscordRest* to add the @NetCord.Rest.RestClient and then configure either HTTP interactions or webhook events. Choose the appropriate tab for your preferred event type below.

### [Http Interactions](#tab/http-interactions)

To handle HTTP interactions from Discord, call @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseHttpInteractions* to map the HTTP interactions route. You can also use @NetCord.Hosting.Services.ApplicationCommands.ApplicationCommandServiceServiceCollectionExtensions.AddHttpApplicationCommands* to add the application command service with preconfigured HTTP contexts to your host builder.

[!code-cs[Program.cs](Introduction.HttpInteractions/Program.cs?highlight=8,16)]

You can also create your own @NetCord.Hosting.IHttpInteractionHandler to handle HTTP interactions manually. This allows you to have full control over the behavior of your bot when receiving HTTP interactions. You register them using @NetCord.Hosting.HttpInteractionHandlerServiceCollectionExtensions.AddHttpInteractionHandler*.

[!code-cs[HttpInteractionHandler.cs](Introduction.HttpInteractions/HttpInteractionHandler.cs#L6-L14)]

#### Configuring Your Discord Bot for HTTP Interactions

To make your bot receive HTTP interactions from Discord, you need to store the public key in the configuration and specify the endpoint URL in the [Discord Developer Portal](https://discord.com/developers/applications).

![Shows 'Public Key' and 'Interaction Endpoint URL' sections in 'General Information' section](../../images/http-events_FindingPublicKeyAndSpecifyingInteractionEndpointUrl.webp){width=850px}

##### Specifying the Public Key in the Configuration

You can for example use `appsettings.json` file for configuration. It should look like this:

[!code-json[appsettings.json](Introduction.HttpInteractions/appsettings.json?highlight=4)]

##### Specifying the Interactions Endpoint URL

If your bot is hosted at `https://example.com` and you have specified `/interactions` pattern in @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseHttpInteractions*, the endpoint URL will be `https://example.com/interactions`. Also note that Discord sends validation requests to the endpoint URL, so your bot must be running while updating it.

### [Webhook Events](#tab/webhook-events)

To handle webhook events from Discord, call @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseWebhookEvents* to map the webhook events route and use @NetCord.Hosting.AspNetCore.WebhookHandlerServiceCollectionExtensions.AddWebhookHandler* to register handlers for specific webhook events.

[!code-cs[Program.cs](Introduction.WebhookEvents/Program.cs?highlight=8,20)]

You can also create your own webhook handler classes that implement event-specific interfaces like @NetCord.Hosting.AspNetCore.IApplicationAuthorizedWebhookHandler. You register them using @NetCord.Hosting.AspNetCore.WebhookHandlerServiceCollectionExtensions.AddWebhookHandler* as well.

[!code-cs[WebhookEventHandler.cs](Introduction.WebhookEvents/ApplicationAuthorizedWebhookHandler.cs)]

#### Configuring your app for Webhook Events

To make your app receive Webhook Events from Discord, you need to store the public key in the configuration, you can find it in the [Discord Developer Portal](https://discord.com/developers/applications).
![Shows 'Public Key' section in 'General Information' section](../../images/http-events_FindingPublicKey.webp){width=850px}

##### Specifying the Public Key in the Configuration

You can for example use `appsettings.json` file for configuration. It should look like this:

[!code-json[appsettings.json](Introduction.HttpInteractions/appsettings.json?highlight=4)]

##### Enabling Webhook Events

You need to specify the endpoint URL and enable Webhook Events in the [Discord Developer Portal](https://discord.com/developers/applications) to make your app receive Webhook Events.
![Shows 'Public Key' section in 'General Information' section](../../images/http-events_SpecifyingWebhookEndpointAndEnablingWebhookEvents.webp){width=850px}

If your bot is hosted at `https://example.com` and you have specified `/webhooks` pattern in @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseWebhookEvents*, the endpoint URL will be `https://example.com/webhooks`. Also note that Discord sends validation requests to the endpoint URL, so your app must be running while updating it.

***

### Local Testing with ngrok

For local testing, you can use [ngrok](https://ngrok.com), a tool that exposes your local server to the internet, providing a public URL to receive events. Use the following command to start ngrok with a correct port specified:

```bash
ngrok http http://localhost:port
```

It will generate a URL that you can use to receive events from Discord. For example, if the URL is `https://random-subdomain.ngrok-free.app`:

- For **HTTP Interactions** with pattern `/interactions`, the endpoint URL will be `https://random-subdomain.ngrok-free.app/interactions`.
- For **Webhook Events** with pattern `/webhooks`, the endpoint URL will be `https://random-subdomain.ngrok-free.app/webhooks`.

## Extending Your Bot

Now, as you have your bot up and running, you can start adding more features to it.

- **[Application Commands](../services/application-commands/introduction.md):** Learn how to make complex commands with parameters and subcommands with ease (HTTP Interactions only).
- **[Component Interactions](../services/component-interactions/introduction.md):** Make your bot interactive with buttons, select menus, and other components easily (HTTP Interactions only).
