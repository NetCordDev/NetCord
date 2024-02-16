using NetCord;
using NetCord.Gateway;
using NetCord.Services.Commands;

namespace MyBot;

public class CustomCommandContext(Message message, GatewayClient client) : CommandContext(message, client)
{
    public GuildUser BotGuildUser => Guild!.Users[Client.Id];
}
