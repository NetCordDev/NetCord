# Coding!

> [!Note]
> This section assumes that you have a project with NetCord installed and you have a bot created. If not, go back!

First, add the following line to file `Program.cs`.
```cs
using NetCord;
using NetCord.Gateway;
```

Now, it's time to create a bot instance! But before, you need a token of your bot... so you need to return to [Discord Developer Portal](https://discord.com/developers/applications) and get it.
![](../../images/coding_Token_1.png)
![](../../images/coding_Token_2.png)

> [!IMPORTANT]
> You should never give your token to anybody.

Add the following line to the file.
```cs
GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"));
```

Logging is important, because of it you know what is your bot doing. To add it, add the following lines to your code.
```cs
client.Log += message =>
{
	Console.WriteLine(message);
	return default;
};
```

Now it's time to finally... make the bot online! To do it, add the following lines to your code.
```cs
await client.StartAsync();
await Task.Delay(-1);
```

Now, when you run the code, your bot should be online!
![](../../images/coding_BotOnline.png)

If not, check the console window, you probably have the following message: `Disconnected: Authentication failed`. If yes, check if your token is correct.

## The Final Product
```cs
using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"));
client.Log += message =>
{
	Console.WriteLine(message);
	return default;
};
await client.StartAsync();
await Task.Delay(-1);
```