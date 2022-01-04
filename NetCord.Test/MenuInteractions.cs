using NetCord.Interactions;

namespace NetCord.Test
{
    public class MenuInteractions : MenuInteractionModule
    {
        [Interaction("roles")]
        public async Task Roles()
        {
            var user = Context.User;
            if (user is GuildUser guildUser)
            {
                var selectedValues = Context.Interaction.Data.SelectedValues.Select(s => new DiscordId(s));
                await guildUser.ModifyAsync(x => x.NewRolesIds = selectedValues);
                await Context.Interaction.EndWithReplyAsync(new InteractionMessage { Content = "Roles updated" });
            }
            else
                await Context.Interaction.EndWithReplyAsync(new() { Content = "You are not in guild" });
        }

        [Interaction("menu")]
        public Task Menu()
        {
            InteractionMessage interactionMessage = new()
            {
                Ephemeral = true,
                Content = "You selected: " + string.Join(", ", Context.Interaction.Data.SelectedValues),
            };
            return Context.Interaction.EndWithReplyAsync(interactionMessage);
        }
    }
}
