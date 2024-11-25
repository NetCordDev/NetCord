---
uid: sending-messages
omitAppTitle: true
title: Sending Messages in a C# Discord Bot with NetCord Library
description: Learn how to send messages in a C# Discord bot using NetCord, covering text, embeds, attachments, allowed mentions, and interactive components like buttons.
---

# Sending Messages

This guide demonstrates how to send messages to a channel.

Different types of message properties are available because various endpoints support different options. However, all these types implement @NetCord.Rest.IMessageProperties, which defines the properties supported by all endpoints. This guide will cover the properties defined in @NetCord.Rest.IMessageProperties.

## Types of Message Properties

The following message properties types are available:
- @NetCord.Rest.MessageProperties - Can be used with @"NetCord.Rest.RestClient.SendMessageAsync*?text=RestClient.SendMessageAsync" to send messages to channels.
- @NetCord.Rest.InteractionMessageProperties - Used for sending messages in response to interactions. Refer to @"responding-to-interactions?text=Responding to Interactions#responding-with-a-message" for more information.
- @NetCord.Rest.ReplyMessageProperties - Can be used with @"NetCord.Rest.RestMessage.ReplyAsync*?text=RestMessage.ReplyAsync" to reply to messages.
- @NetCord.Rest.WebhookMessageProperties - Can be used with @"NetCord.Rest.RestClient.ExecuteWebhookAsync*?text=RestClient.ExecuteWebhookAsync" to send messages via webhooks.
- @NetCord.Rest.ForumGuildThreadMessageProperties - Can be used with @"NetCord.Rest.RestClient.CreateForumGuildThreadAsync*?text=RestClient.CreateForumGuildThreadAsync" to create forum posts.

All the @NetCord.Rest.RestClient methods mentioned above have their equivalent methods in appropriate objects, such as @NetCord.TextChannel and @NetCord.Rest.WebhookClient.

## Using @NetCord.Rest.IMessageProperties

If you need to build an API that supports creating messages for multiple endpoints, you can utilize the @NetCord.Rest.IMessageProperties interface. This allows you to define the properties common to all message properties types in a straightforward manner.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Using 'IMessageProperties'](SendingMessages/Program.cs#L283-L290)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Using 'IMessageProperties'](SendingMessages/Program.cs#L295-L304)]

***

You can view sample usages of this method below:

[!code-cs[Using our API for 'MessageProperties'](SendingMessages/Program.cs#L310)]
[!code-cs[Using our API for 'InteractionMessageProperties'](SendingMessages/Program.cs#L312)]

## Defining the Content

The content refers to the main text of the message.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Defining the content](SendingMessages/Program.cs#L10)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Defining the content](SendingMessages/Program.cs#L12)]

***

All message properties types support an implicit conversion from a string. However, note that this does not apply to @NetCord.Rest.IMessageProperties, as it is an interface.

[!code-cs[Implicit conversion from string](SendingMessages/Program.cs#L278)]

## Adding Embeds

Embeds are rich content elements that can be attached to a message. They support titles, descriptions, images, and more. A message can include up to 10 embeds.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Adding embeds](SendingMessages/Program.cs#L96)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Adding embeds](SendingMessages/Program.cs#L98)]

***

### Customizing Embeds

You can customize embeds by specifying properties such as the title, description, color, and image.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Customizing embeds](SendingMessages/Program.cs#L16-L62)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Customizing embeds](SendingMessages/Program.cs#L64-L94)]

***

## Managing Allowed Mentions

Allowed mentions define which users and roles can be mentioned within the message.

To allow all mentions, use @"NetCord.Rest.AllowedMentionsProperties.All?text=AllowedMentionsProperties.All".

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Setting allowed mentions](SendingMessages/Program.cs#L100)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Setting allowed mentions](SendingMessages/Program.cs#L102)]

***

To disable all mentions, use @"NetCord.Rest.AllowedMentionsProperties.None?text=AllowedMentionsProperties.None".

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Setting allowed mentions](SendingMessages/Program.cs#L104)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Setting allowed mentions](SendingMessages/Program.cs#L106)]

***

You can also define custom allowed mentions.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Customizing allowed mentions](SendingMessages/Program.cs#L108-L114)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Customizing allowed mentions](SendingMessages/Program.cs#L116-L120)]

***

## Attaching Files

Attachments are files that can be added to a message. A message can contain up to 10 attachments.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Adding attachments](SendingMessages/Program.cs#L153)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Adding attachments](SendingMessages/Program.cs#L155)]

***

### Customizing Attachments

Attachments are files that can be added to a message, with up to 10 attachments per message. All attachment types support any readable stream, such as file streams. Below are examples of how you can create and customize attachments.

#### Standard Attachments

This is an example of a standard attachment. The following code creates an attachment with the content "Hello!".

[!code-cs[Creating attachments](SendingMessages/Program.cs#L124)]

#### Base64-Encoded Attachments

You can create an attachment with base64 encoding. The following example demonstrates an attachment that will display "Hello, base64!" in the chat.

[!code-cs[Base64 attachment](SendingMessages/Program.cs#L126)]

#### Quoted-Printable Attachments

Quoted-printable encoding is another supported format. The following example creates an attachment that will display "Różowy means pink" in the chat.

[!code-cs[Quoted-printable attachment](SendingMessages/Program.cs#L128-L129)]

#### Google Cloud Attachments

Discord supports uploading attachments directly to Google Cloud Platform. This example creates an attachment uploaded directly to Google Cloud Platform with the content "Hello, Google!".

[!code-cs[Google Cloud attachment](SendingMessages/Program.cs#L135-L144)]

#### Adding Titles and Descriptions

You can enhance your attachments by adding titles and descriptions. Use the `AttachmentProperties.Title` and `AttachmentProperties.Description` properties to set these values. Here's an example:

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Attachment properties](SendingMessages/Program.cs#L146-L147)] 

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Attachment properties](SendingMessages/Program.cs#L149-L151)] 

***

## Adding Components

Components are interactive elements that can be attached to a message. These include buttons, select menus, and more. A message can contain up to 5 components, including action rows and select menus.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Adding components](SendingMessages/Program.cs#L251)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Adding components](SendingMessages/Program.cs#L253)]

***

### Action Rows

Action rows contain buttons, and each can have up to 5 buttons. Available button types include:
- @NetCord.Rest.ButtonProperties, which triggers an interaction when clicked.
- @NetCord.Rest.LinkButtonProperties, which opens a URL when clicked.
- @NetCord.Rest.PremiumButtonProperties, which prompts the user to pay when clicked.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Defining action rows](SendingMessages/Program.cs#L159-L169)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Defining action rows](SendingMessages/Program.cs#L171-L178)]

***

### Select Menus

Select menus are dropdown menus containing up to 25 options. They support various types, such as strings, channels, and users.

#### String Menus

String menus allow you to include any string options. Each option in the menu can be customized with additional properties, such as setting a default selection, adding an emoji, or providing a description for better context.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Creating string menus](SendingMessages/Program.cs#L180-L198)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Creating string menus](SendingMessages/Program.cs#L200-L211)]

***

#### Channel Menus

Channel menus include channels as options, and support filtering by channel type and the ability to specify default channels.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Creating channel menus](SendingMessages/Program.cs#L213-L217)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Creating channel menus](SendingMessages/Program.cs#L219-L221)]

***

#### Mentionable Menus

Mentionable menus include users and roles as options, and support specifying default users and roles.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Creating mentionable menus](SendingMessages/Program.cs#L223-L229)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Creating mentionable menus](SendingMessages/Program.cs#L231-L233)]

***

#### Role Menus

Role menus contain roles as options, and support specifying default roles.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Creating role menus](SendingMessages/Program.cs#L235-L238)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Creating role menus](SendingMessages/Program.cs#L240-L241)]

***

#### User Menus

User menus allow selecting users as options, and support specifying default users.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Creating user menus](SendingMessages/Program.cs#L243-L246)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Creating user menus](SendingMessages/Program.cs#L248-L249)]

***

#### Specifying Additional Properties

Additionally, all select menus allow you to specify a placeholder, set a minimum and maximum number of selectable options, and disable the select menu if necessary.

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Configuring select menus](SendingMessages/Program.cs#L264-L267)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Configuring select menus](SendingMessages/Program.cs#L269-L273)]

***

## Configuring Flags

Flags control how a message behaves. You can combine multiple flags using the bitwise OR operator.

For example, this configuration specifies that no embeds from URLs should be displayed, and the message should be silent (not triggering push or desktop notifications).

# [Classic Syntax](#tab/classic-syntax)

[!code-cs[Setting flags](SendingMessages/Program.cs#L255)]

# [Fluent Syntax](#tab/fluent-syntax)

[!code-cs[Setting flags](SendingMessages/Program.cs#L257)]

***
