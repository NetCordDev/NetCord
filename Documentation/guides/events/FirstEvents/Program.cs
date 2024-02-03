using NetCord;
using NetCord.Gateway;

GatewayClient client = new(new BotToken("Token from Discord Developer Portal"), new GatewayClientConfiguration()
{
    Intents = GatewayIntents.GuildMessages
              | GatewayIntents.DirectMessages
              | GatewayIntents.MessageContent
              | GatewayIntents.DirectMessageReactions
              | GatewayIntents.GuildMessageReactions,
});

client.MessageCreate += message =>
{
    Console.WriteLine(message.Content);
    return default;
};

client.MessageReactionAdd += async args =>
{
    await client.Rest.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
};
