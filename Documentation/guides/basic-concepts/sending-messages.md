---
uid: sending-messages
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

[!code-cs[Using 'IMessageProperties'](SendingMessages/Program.cs#L6-L13)]

You can view sample usages of this method below:

[!code-cs[Using our API for 'MessageProperties'](SendingMessages/Program.cs#L19)]
[!code-cs[Using our API for 'InteractionMessageProperties'](SendingMessages/Program.cs#L21)]

## Defining the Content

The content refers to the main text of the message.

[!code-cs[Defining the content](SendingMessages/Program.cs#L28)]

All message properties types support an implicit conversion from a string. However, note that this does not apply to @NetCord.Rest.IMessageProperties, as it is an interface.

[!code-cs[Implicit conversion from string](SendingMessages/Program.cs#L198)]

## Adding Embeds

Embeds are rich content elements that can be attached to a message. They support titles, descriptions, images, and more. A message can include up to 10 embeds.

[!code-cs[Adding embeds](SendingMessages/Program.cs#L80)]

### Customizing Embeds

You can customize embeds by specifying properties such as the title, description, color, and image.

[!code-cs[Customizing embeds](SendingMessages/Program.cs#L32-L78)]

## Managing Allowed Mentions

Allowed mentions define which users and roles can be mentioned within the message.

To allow all mentions, use @"NetCord.Rest.AllowedMentionsProperties.All?text=AllowedMentionsProperties.All".

[!code-cs[Setting allowed mentions](SendingMessages/Program.cs#L82)]

To disable all mentions, use @"NetCord.Rest.AllowedMentionsProperties.None?text=AllowedMentionsProperties.None".

[!code-cs[Disabling allowed mentions](SendingMessages/Program.cs#L84)]

You can also define custom allowed mentions.

[!code-cs[Customizing allowed mentions](SendingMessages/Program.cs#L86-L92)]

## Attaching Files

Attachments are files that can be added to a message. A message can contain up to 10 attachments.

[!code-cs[Adding attachments](SendingMessages/Program.cs#L121)]

### Customizing Attachments

Attachments are files that can be added to a message, with up to 10 attachments per message. All attachment types support any readable stream, such as file streams. Below are examples of how you can create and customize attachments.

#### Standard Attachments

This is an example of a standard attachment. The following code creates an attachment with the content "Hello!".

[!code-cs[Creating attachments](SendingMessages/Program.cs#L96)]

#### Base64-Encoded Attachments

You can create an attachment with base64 encoding. The following example demonstrates an attachment that will display "Hello, base64!" in the chat:

[!code-cs[Base64 attachment](SendingMessages/Program.cs#L98)]

#### Quoted-Printable Attachments

Quoted-printable encoding is another supported format. The following example creates an attachment that will display "Różowy means pink" in the chat:

[!code-cs[Quoted-printable attachment](SendingMessages/Program.cs#L100-L101)]

#### Google Cloud Attachments

Discord supports uploading attachments directly to Google Cloud Platform. This example creates an attachment uploaded directly to Google Cloud Platform with the content "Hello, Google!":

[!code-cs[Google Cloud attachment](SendingMessages/Program.cs#L107-L116)]

#### Adding Titles and Descriptions

You can enhance your attachments by adding titles and descriptions. Use the `AttachmentProperties.Title` and `AttachmentProperties.Description` properties to set these values. Here's an example:

[!code-cs[Attachment properties](SendingMessages/Program.cs#L118-L119)] 

## Adding Components

Components are interactive elements that can be attached to a message. These include buttons, select menus, and more. A message can contain up to 5 components, including action rows and select menus.

[!code-cs[Adding components](SendingMessages/Program.cs#L181)]

### Action Rows

Action rows contain buttons, and each can have up to 5 buttons. Available button types include:
- @NetCord.Rest.ButtonProperties, which triggers an interaction when clicked.
- @NetCord.Rest.LinkButtonProperties, which opens a URL when clicked.
- @NetCord.Rest.PremiumButtonProperties, which prompts the user to pay when clicked.

[!code-cs[Defining action rows](SendingMessages/Program.cs#L125-L135)]

### Select Menus

Select menus are dropdown menus containing up to 25 options. They support various types, such as strings, channels, and users.

#### String Menus

String menus allow you to include any string options. Each option in the menu can be customized with additional properties, such as setting a default selection, adding an emoji, or providing a description for better context.

[!code-cs[Creating string menus](SendingMessages/Program.cs#L137-L155)]

#### Channel Menus

Channel menus include channels as options, and support filtering by channel type and the ability to specify default channels.

[!code-cs[Creating channel menus](SendingMessages/Program.cs#L157-L161)]

#### Mentionable Menus

Mentionable menus include users and roles as options, and support specifying default users and roles.

[!code-cs[Creating mentionable menus](SendingMessages/Program.cs#L163-L169)]

#### Role Menus

Role menus contain roles as options, and support specifying default roles.

[!code-cs[Creating role menus](SendingMessages/Program.cs#L171-L174)]

#### User Menus

User menus allow selecting users as options, and support specifying default users.

[!code-cs[Creating user menus](SendingMessages/Program.cs#L176-L179)]

***

Additionally, all select menus allow you to specify a placeholder, set a minimum and maximum number of selectable options, and disable the select menu if necessary.

[!code-cs[Configuring select menus](SendingMessages/Program.cs#L190-L193)]

## Configuring Flags

Flags control how a message behaves. You can combine multiple flags using the bitwise OR operator.

For example, this configuration specifies that no embeds from URLs should be displayed, and the message should be silent (not triggering push or desktop notifications).

[!code-cs[Setting flags](SendingMessages/Program.cs#L183)]
