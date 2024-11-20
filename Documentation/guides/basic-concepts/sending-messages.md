---
uid: sending-messages
---

# Sending Messages

This guide will show you how to send messages to a channel.

There are many message properties as different endpoints support different values. Though they all implement @NetCord.Rest.IMessageProperties which contains the most common properties, and these properties will be described in this guide.

## Making use of @NetCord.Rest.IMessageProperties

Sometimes you may want to write an API that will support creating messages for multiple endpoints. In this case, you can use the @NetCord.Rest.IMessageProperties interface to define the properties that are common to all endpoints easily.

[!code-cs[Making use of 'IMessageProperties'](SendingMessages/Program.cs#L14-L21)]

You can see sample usages of this method here.

[!code-cs[Using our API for 'MessageProperties'](SendingMessages/Program.cs#L25)]
[!code-cs[Using our API for 'InteractionMessageProperties'](SendingMessages/Program.cs#L27)]

## Specifying the Content

The content is the main text of the message.

[!code-cs[Specifying the content](SendingMessages/Program.cs#L34)]

All message properties also have an implicit conversion from string. Note that this won't work for @NetCord.Rest.IMessageProperties as it is an interface.

[!code-cs[Implicit conversion from string](SendingMessages/Program.cs#L96)]

## Specifying Embeds

Embeds are rich content that can be attached to a message. They can contain titles, descriptions, images, and more. You can add up to 10 embeds to a message.

[!code-cs[Specifying embeds](SendingMessages/Program.cs#L43)]

### Customizing Embeds

## Specifying Allowed Mentions

Allowed mentions control which users and roles can be mentioned in the message.

Using @"NetCord.Rest.AllowedMentionsProperties.All?text=AllowedMentionsProperties.All" will allow all mentions.

[!code-cs[Specifying allowed mentions](SendingMessages/Program.cs#L45)]

While using @"NetCord.Rest.AllowedMentionsProperties.None?text=AllowedMentionsProperties.None" will disable all mentions.

[!code-cs[Specifying allowed mentions](SendingMessages/Program.cs#L47)]

You can also create your own custom allowed mentions.

[!code-cs[Specifying allowed mentions](SendingMessages/Program.cs#L49-L55)]

## Specifying Attachments

Attachments are files that can be attached to a message. You can add up to 10 attachments to a message.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L85)]

### Customizing Attachments

A classic example of attachment creation can be. You can of course specify any type of stream that is readable, such as a file stream.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L59)]

You can also create attachments in different formats like for example in Base64.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L61)]

Or with quoted-printable encoding.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L63-L64)]

Discord also supports uploading attachments directly into the Google Cloud Platform.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L70-L79)]

You can also specify @"NetCord.Rest.AttachmentProperties.Title?text=AttachmentProperties.Title" and @"NetCord.Rest.AttachmentProperties.Description?text=AttachmentProperties.Description" to specify the title and description of the attachment.

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L81)]

[!code-cs[Specifying attachments](SendingMessages/Program.cs#L83)]

## Specifying Components

## Specifying Flags

Flags control the behavior of the message. You can specify multiple flags by combining them with the bitwise OR operator.

This example specifies that the message should not display any embeds from URLs it contains and that it should be silent (shouldn't trigger push and desktop notifications).

[!code-cs[Specifying flags](SendingMessages/Program.cs#L91)]
