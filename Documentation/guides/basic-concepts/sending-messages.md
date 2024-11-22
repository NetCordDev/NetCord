---
uid: sending-messages
---

# Sending Messages

This guide demonstrates how to send messages to a channel.

Different message properties types are available since various endpoints support different values. However, all the types implement @NetCord.Rest.IMessageProperties, which includes the most commonly used properties. The properties of @NetCord.Rest.IMessageProperties will be covered in this guide.

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

[!code-cs[Implicit conversion from string](SendingMessages/Program.cs#L199)]

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

[!code-cs[Adding attachments](SendingMessages/Program.cs#L122)]

### Customizing Attachments

> [!NOTE]
> Any readable stream can be used to create an attachment, such as a file stream.

A classic example of attachment creation is shown below.

[!code-cs[Creating attachments](SendingMessages/Program.cs#L96)]

Attachments can also be created in different formats. In this example, a base64-encoded attachment will appear as "Hello, base64!" in the chat.

[!code-cs[Base64 attachment](SendingMessages/Program.cs#L98)]

Another example is an attachment with quoted-printable encoding, which will display as "Różowy means pink" in the chat.

[!code-cs[Quoted-printable attachment](SendingMessages/Program.cs#L100-L101)]

Discord also supports uploading attachments directly to Google Cloud Platform. The following example will create an attachment with "Hello, Google!" content.

[!code-cs[Google Cloud attachment](SendingMessages/Program.cs#L107-L116)]

Additionally, you can set the title and description of any attachment using @"NetCord.Rest.AttachmentProperties.Title?text=AttachmentProperties.Title" and @"NetCord.Rest.AttachmentProperties.Description?text=AttachmentProperties.Description".

[!code-cs[Attachment properties](SendingMessages/Program.cs#L118)]

[!code-cs[Another attachment example](SendingMessages/Program.cs#L120)]

## Adding Components

Components are interactive elements that can be attached to a message. These include buttons, select menus, and more. A message can contain up to 5 components, including action rows and select menus.

[!code-cs[Adding components](SendingMessages/Program.cs#L182)]

### Action Rows

Action rows contain buttons, and each can have up to 5 buttons. Available button types include:
- @NetCord.Rest.ButtonProperties, which triggers an interaction when clicked.
- @NetCord.Rest.LinkButtonProperties, which opens a URL when clicked.
- @NetCord.Rest.PremiumButtonProperties, which prompts the user to pay when clicked.

[!code-cs[Defining action rows](SendingMessages/Program.cs#L126-L136)]

### Select Menus

Select menus are dropdown menus containing up to 25 options. They support various types, such as strings, channels, and users.

#### String Menus

String menus allow you to include any string options. Each option in the menu can be customized with additional properties, such as setting a default selection, adding an emoji, or providing a description for better context.

[!code-cs[Creating string menus](SendingMessages/Program.cs#L138-L156)]

#### Channel Menus

Channel menus include channels as options, and support filtering by channel type and the ability to specify default channels.

[!code-cs[Creating channel menus](SendingMessages/Program.cs#L158-L162)]

#### Mentionable Menus

Mentionable menus include users and roles as options, and support specifying default users and roles.

[!code-cs[Creating mentionable menus](SendingMessages/Program.cs#L164-L170)]

#### Role Menus

Role menus contain roles as options, and support specifying default roles.

[!code-cs[Creating role menus](SendingMessages/Program.cs#L172-L175)]

#### User Menus

User menus allow selecting users as options, and support specifying default users.

[!code-cs[Creating user menus](SendingMessages/Program.cs#L177-L180)]

***

Additionally, all select menus allow you to specify a placeholder, set a minimum and maximum number of selectable options, and disable the select menu if necessary.

[!code-cs[Configuring select menus](SendingMessages/Program.cs#L191-L194)]

## Configuring Flags

Flags control how a message behaves. You can combine multiple flags using the bitwise OR operator.

For example, this configuration specifies that no embeds from URLs should be displayed, and the message should be silent (not triggering push or desktop notifications).

[!code-cs[Setting flags](SendingMessages/Program.cs#L184)]
