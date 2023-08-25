# Coding!

> [!Note]
> This section assumes that you have a project with NetCord installed and you have a bot created. If not, go back!

First, add the following line to file `Program.cs`.
[!code-cs[Program.cs](Coding/Program.cs#L1-L2)]

Now, it's time to create a bot instance! But before, you need a token of your bot... so you need to return to [Discord Developer Portal](https://discord.com/developers/applications) and get it.
![](../../images/coding_Token_1.png)
![](../../images/coding_Token_2.png)

> [!IMPORTANT]
> You should never give your token to anybody.

Add the following line to the file.
[!code-cs[Program.cs](coding/Program.cs#L4)]

Logging is important, because of it you know what is your bot doing. To add it, add the following lines to your code.
[!code-cs[Program.cs](coding/Program.cs#L6-L10)]

Now it's time to finally... make the bot online! To do it, add the following lines to your code.
[!code-cs[Program.cs](coding/Program.cs#L12-L13)]

Now, when you run the code, your bot should be online!
![](../../images/coding_BotOnline.png)

If not, check the console window, you will probably see the following message: `Disconnected: Authentication failed`. If so, check that your token is correct.

## The Final Product
[!code-cs[Program.cs](Coding/Program.cs)]