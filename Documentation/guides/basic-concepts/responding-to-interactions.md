---
uid: responding-to-interactions
omitAppTitle: true
title: Responding to Interactions in C# Discord Bots with NetCord
description: Learn to handle interactions in your C# Discord bot with NetCord. Explore response types like messages, modals, and deferrals for building bots with .NET.
---

# Responding to Interactions

This guide explains how to handle interactions and prepare appropriate responses.

To create a response, use the @NetCord.Rest.InteractionCallback class. It supports various response types, such as messages, modals, and autocomplete suggestions. Once created, the callback must be passed to the @"NetCord.Interaction.SendResponseAsync*?text=Interaction.SendResponseAsync" method to send it to Discord.

[!code-cs[Sample response](RespondingToInteractions/Program.cs#L15)]

The [NetCord.Services](https://www.nuget.org/packages/NetCord.Services) package provides additional utilities to simplify this process.

## Responding with a Message

You can respond with a message by using @"NetCord.Rest.InteractionCallback.Message*?text=InteractionCallback.Message".

[!code-cs[Responding with a message](RespondingToInteractions/Program.cs#L22)]

For advanced message options, see @"sending-messages?text=Sending Messages".

## Responding with Deferral

Deferring is useful when performing long-running operations before sending a message. Deferrals give you up to 15 minutes to complete the operation. After deferring, you can either modify the initial response (the deferral) or send a follow-up message, both work the same way.

[!code-cs[Responding after deferral](RespondingToInteractions/Program.cs#L49)]
[!code-cs[Responding after deferral](RespondingToInteractions/Program.cs#L51)]

The [NetCord.Services](https://www.nuget.org/packages/NetCord.Services) package also provides shortcuts for deferral handling.

For advanced message options, see @"sending-messages?text=Sending Messages".

### Deferred Message

For application commands, use @"NetCord.Rest.InteractionCallback.DeferredMessage*?text=InteractionCallback.DeferredMessage" to send a deferral response. This shows a loading state to the user while you prepare the message.

[!code-cs[Responding with a deferred message](RespondingToInteractions/Program.cs#L24)]

You can specify @"NetCord.MessageFlags.Ephemeral?text=MessageFlags.Ephemeral" to make the response visible only to the user.

[!code-cs[Responding with a deferred message](RespondingToInteractions/Program.cs#L26)]

### Deferred Modify Message

For component interactions, use @"NetCord.Rest.InteractionCallback.DeferredModifyMessage?text=InteractionCallback.DeferredModifyMessage". This type of deferral doesn't display a loading state to the user.

[!code-cs[Responding with a deferred modify message](RespondingToInteractions/Program.cs#L28)]

## Responding with Message Modification

For message component interactions, you can modify the message they are attached to using @"NetCord.Rest.InteractionCallback.ModifyMessage*?text=InteractionCallback.ModifyMessage".

[!code-cs[Responding with a modify message](RespondingToInteractions/Program.cs#L30)]

## Responding with a Modal

Modals are interactive forms users can fill out. Use @"NetCord.Rest.InteractionCallback.Modal*?text=InteractionCallback.Modal" to create a modal. Each modal can include up to five text inputs, but no other component types are supported.

[!code-cs[Responding with a modal](RespondingToInteractions/Program.cs#L32-L36)]

## Responding with Autocomplete

Autocomplete provides a list of options while the user types a slash command. Use @"NetCord.Rest.InteractionCallback.Autocomplete*?text=InteractionCallback.Autocomplete" to respond to autocomplete interactions.

For string parameters:

[!code-cs[Responding with autocomplete](RespondingToInteractions/Program.cs#L38)]

For numeric parameters:

[!code-cs[Responding with autocomplete](RespondingToInteractions/Program.cs#L40)]

## Responding with a Pong

A "pong" callback is used to respond to ping HTTP interactions sent by Discord to verify the endpoint's availability. Use @"NetCord.Rest.InteractionCallback.Pong?text=InteractionCallback.Pong" to create this response.

[!code-cs[Responding with a pong](RespondingToInteractions/Program.cs#L42)]

If you're using the [NetCord.Hosting.AspNetCore](https://www.nuget.org/packages/NetCord.Hosting.AspNetCore) package, these ping interactions are handled automatically, and you don't need to provide the callback manually.
