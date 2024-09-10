using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class MentionableMenuModule : ComponentInteractionModule<MentionableMenuInteractionContext>
{
    [ComponentInteraction("menu")]
    public string Menu() => $"You selected: {string.Join(", ", Context.SelectedMentionables)}";
}
