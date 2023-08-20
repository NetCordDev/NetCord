using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new Token(TokenType.Bot, "Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.MessageContent,
});

client.MessageCreate += message =>
{
    Console.WriteLine(message.Content);
    return default;
};

client.MessageUpdate += async message =>
{
    await message.ReplyAsync("This message was modified!");
};
