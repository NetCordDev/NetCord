using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class MessageCommandModule : ApplicationCommandModule<MessageCommandContext>
{
    [MessageCommand("Timestamp")]
    public string Timestamp() => Context.Target.CreatedAt.ToString();
}
