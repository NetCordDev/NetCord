using System.Text;

using NetCord.Services.Interactions;

namespace NetCord.Test;

public class MentionableMenuInteractions : InteractionModule<MentionableMenuInteractionContext>
{
    [Interaction("mentionables")]
    public Task MentionablesAsync()
    {
        StringBuilder stringBuilder = new("You selected: ");
        foreach (var m in Context.SelectedMentionables.Values)
        {
            if (m.Type == Services.MentionableType.User)
                stringBuilder.Append(m.User);
            else
                stringBuilder.Append(m.Role);
            stringBuilder.Append(", ");
        }
        if (Context.SelectedMentionables.Count != 0)
            stringBuilder.Length -= 2;
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(stringBuilder.ToString()));
    }
}
