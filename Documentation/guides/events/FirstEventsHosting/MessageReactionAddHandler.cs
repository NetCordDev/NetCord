using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;

namespace MyBot;

public class MessageReactionAddHandler(RestClient client) : IMessageReactionAddGatewayHandler
{
    public async ValueTask HandleAsync(MessageReactionAddEventArgs args)
    {
        await client.SendMessageAsync(args.ChannelId, $"<@{args.UserId}> reacted with {args.Emoji.Name}!");
    }
}
