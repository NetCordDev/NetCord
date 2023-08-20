using NetCord;
using NetCord.Services.Commands;

namespace MyBot;

public abstract class CustomCommandModule : CommandModule<CommandContext>
{
    public Color GetUserColor(GuildUser user)
    {
        return (user.GetRoles(Context.Guild!)
                    .OrderByDescending(r => r.Position)
                    .FirstOrDefault(r => r.Color != default)?.Color).GetValueOrDefault();
    }
}
