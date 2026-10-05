using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace NetCord.Test;

public class StringSelectInteractions : BaseComponentInteractionModule<StringSelectInteractionContext>
{
    [ComponentInteraction("roles")]
    public async Task Roles()
    {
        var user = Context.User;
        if (user is GuildUser guildUser)
        {
            var values = Context.Interaction.Data.Values.Select(s => Snowflake.Parse(s));
            await guildUser.ModifyAsync(x => x.RoleIds = values);
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = "Roles updated" }));
        }
        else
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = "You are not in guild" }));
    }

    [ComponentInteraction("select")]
    public Task Select(string s)
    {
        _ = s;
        InteractionMessageProperties interactionMessage = new()
        {
            Flags = MessageFlags.Ephemeral,
            Content = "You selected: " + string.Join(", ", Context.Values),
        };
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(interactionMessage));
    }
}
