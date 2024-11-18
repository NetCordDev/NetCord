using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace MyBot;

public class ExampleModule : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";

    [UserCommand("ID")]
    public static string Id(User user) => user.Id.ToString();

    [MessageCommand("Timestamp")]
    public static string Timestamp(RestMessage message) => message.CreatedAt.ToString();
}
