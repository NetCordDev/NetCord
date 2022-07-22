# First Events

> [!WARNING]
> You need to enable `MessageContentIntent` to receive @NetCord.Rest.RestMessage.Content of message. If you don't know how, go to @"Intents".

## MessageCreate Event
@NetCord.Gateway.GatewayClient.MessageCreate event fires when a message is sent. To detect it, add the following lines before `client.StartAsync()`.
```cs
client.MessageCreate += message =>
{
	Console.WriteLine(message.Content);
	return default;
};
```
When you run this code, when you send a message, the message will be printed on a console.

## MessageUpdate Event
@NetCord.Gateway.GatewayClient.MessageUpdate event fires when a message is modified. To detect it, add the following lines to your code.
```cs
client.MessageUpdate += async message =>
{
	await message.ReplyAsync("This message was modified!");
};
```
Now your bot is notifying everyone that someone has modified a message!

**Other events work similar to these. You can play with them if you want.**