using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace MyBot;

public class ExampleModule : ComponentInteractionModule<ButtonInteractionContext>
{
    [ComponentInteraction("publish")]
    public static InteractionMessageProperties Publish(string content)
    {
        return new InteractionMessageProperties()
            .AddEmbeds(new EmbedProperties().WithTitle("Publication")
                                            .WithDescription(content)
                                            .WithColor(new Color(0x7777FF)));
    }

    [ComponentInteraction("delete")]
    public async Task<string> DeleteAsync(params ulong[] messageIds)
    {
        await Context.Channel.DeleteMessagesAsync(messageIds);

        return "The messages have been deleted successfully.";
    }

    [ComponentInteraction("unban")]
    public async Task<string> UnbanAsync(ulong userId, string reason = "No reason provided.")
    {
        await Context.Guild!.UnbanUserAsync(userId, new RestRequestProperties().WithAuditLogReason(reason));

        return $"The user has been unbanned successfully.";
    }

    [ComponentInteraction("bug report")]
    public static InteractionMessageProperties BugReport(string title, string body = "No body provided.", string category = "None")
    {
        return new InteractionMessageProperties()
            .AddEmbeds(new EmbedProperties()
                .WithColor(new(0xFF0000))
                .WithTitle(title)
                .WithDescription(body)
                .WithFooter(new EmbedFooterProperties().WithText($"Category: {category}")));
    }
}
