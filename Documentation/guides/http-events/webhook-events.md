---
omitAppTitle: true
title: Handling Discord Webhook Events with C# and ASP.NET Core
description: Learn how to handle Discord Webhook Events using the NetCord.Hosting.AspNetCore package in C# and ASP.NET Core.
---

# Handling Webhook Events with ASP.NET Core

This guide walks you through receiving and handling Webhook Events from Discord in your application using the [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore) package.

## Required Dependencies

Before you get started, make sure you've installed the required native dependencies. Follow the [installation guide](../installing-native-dependencies.md) to set them up.

## Setting Up

To receive Webhook Events from Discord, do the following:

1. Add the @NetCord.Rest.RestClient using @NetCord.Hosting.Rest.RestClientServiceCollectionExtensions.AddDiscordRest*.
2. Map the webhook events route by calling @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseWebhookEvents*.
3. Register handlers for specific webhook events using @NetCord.Hosting.AspNetCore.WebhookHandlerServiceCollectionExtensions.AddWebhookHandler*.

[!code-cs[Program.cs](Introduction.WebhookEvents/Program.cs?highlight=8,20)]

You can inject any DI services your handler needs. To control the lifetime of those injected services, specify a @Microsoft.Extensions.DependencyInjection.ServiceLifetime in the registration method; the default is @Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton. See an example below:

[!code-cs[Delegate-based Webhook Event handler registration with lifetime](Introduction.WebhookEvents/WebhookHandlerExamples.cs#L12-L16)]

Alternatively, you can use a class-based handler. Implement the @NetCord.Hosting.AspNetCore.IWebhookHandler interface of your choice and register it with @NetCord.Hosting.AspNetCore.WebhookHandlerServiceCollectionExtensions.AddWebhookHandler*.

[!code-cs[ApplicationDeauthorizedWebhookHandler.cs](Introduction.WebhookEvents/ApplicationDeauthorizedWebhookHandler.cs#L6-L15)]

Register it as follows:
[!code-cs[Class-based Webhook Handler Registration](Introduction.WebhookEvents/WebhookHandlerExamples.cs#L21)]

To control the lifetime of a single class handler, specify a @Microsoft.Extensions.DependencyInjection.ServiceLifetime in the registration method; the default is @Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton. See an example below:

[!code-cs[Class-based Webhook Handler Registration with lifetime](Introduction.WebhookEvents/WebhookHandlerExamples.cs#L26)]

To register every public class-based handler in an assembly at once, use @NetCord.Hosting.AspNetCore.WebhookHandlerServiceCollectionExtensions.AddWebhookHandlers*.

[!code-cs[Registering all public class-based Webhook Handlers in an assembly](Introduction.WebhookEvents/WebhookHandlerExamples.cs#L31)]

For assembly-wide registration, you can set the lifetime of the discovered handlers by specifying a @Microsoft.Extensions.DependencyInjection.ServiceLifetime in the registration method; the default is @Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton. See an example below:

[!code-cs[Registering all public class-based Webhook Handlers in an assembly with lifetime](Introduction.WebhookEvents/WebhookHandlerExamples.cs#L36)]

### Configuring Webhook Events

To make your app receive Webhook Events from Discord, store the public key in the configuration (you can find it in the [Discord Developer Portal](https://discord.com/developers/applications)), then enable Webhook Events and specify the endpoint URL there.

![Shows 'Public Key' section in 'General Information' section](../../images/http-events_FindingPublicKey.webp){width=850px}

#### Specifying the Public Key in the Configuration

For example, you can use an `appsettings.json` file for configuration. It should look like this:

[!code-json[appsettings.json](Introduction.WebhookEvents/appsettings.json?highlight=4)]

#### Enabling Webhook Events

In the [Discord Developer Portal](https://discord.com/developers/applications), enable Webhook Events to make your app receive them.
![Shows 'Endpoint' and 'Events' sections in 'Webhooks' section](../../images/http-events_SpecifyingWebhookEndpointAndEnablingWebhookEvents.webp){width=850px}

Enable the events you want to receive in the Events section.

### Specifying the Endpoint

The Discord Developer Portal's **Endpoint** field expects the full public URL that Discord will use to reach your app. If your app is hosted at `https://example.com` and you specified the `/webhooks` pattern in @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseWebhookEvents*, that URL is `https://example.com/webhooks`. Discord sends validation requests to the endpoint URL, so your app must be running while you save the change.

For local testing, you can use [ngrok](https://ngrok.com), a tool that exposes your local server to the internet by providing a public URL to receive webhook events. Use the following command to start ngrok on the correct port:
```bash
ngrok http http://localhost:port
```

After it starts, ngrok prints a public URL you can use to receive Webhook Events from Discord. For example, if the URL is `https://random-subdomain.ngrok-free.app` and you specified the `/webhooks` pattern in @NetCord.Hosting.AspNetCore.HttpEventEndpointRouteBuilderExtensions.UseWebhookEvents*, the endpoint URL will be `https://random-subdomain.ngrok-free.app/webhooks`.

## Next Steps

Now that your application is set up to receive Webhook Events, you can expand its features or deploy it to a cloud environment:

- **[AWS Lambda Deployment](aws-lambda.md):** Deploy your application to AWS Lambda for serverless hosting.
- **[Azure Functions Deployment](azure-function.md):** Deploy your application to Azure Functions for serverless hosting.
