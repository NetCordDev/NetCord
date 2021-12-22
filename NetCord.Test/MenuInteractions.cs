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
                await Context.Interaction.EndWithThinkingStateAsync();
                var selectedValues = Context.Interaction.Data.SelectedValues.Select(s => DiscordId.Parse(s));
                await guildUser.ModifyAsync(x => x.NewRolesIds = selectedValues);
                InteractionMessage message = new()
                {
                    Content = "Select roles",
                    Components = new(),
                };
                var menu = NormalCommands.CreateRolesMenu(Context.Guild.Roles.Values, selectedValues);
                message.Components.Add(menu);
                await Context.Interaction.ModifyThinkingStateAsync(new Message { Content = "Roles updated" });
                //await Context.Interaction.EndWithModifyAsync(message.Build());
            }
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
