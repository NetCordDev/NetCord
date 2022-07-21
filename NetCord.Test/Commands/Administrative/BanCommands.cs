using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands.Administrative;

[RequireContext<CommandContext>(Services.RequiredContext.Guild)]
[RequireUserPermission<CommandContext>(Permission.BanUsers), RequireBotPermission<CommandContext>(Permission.BanUsers)]
public class BanCommands : CommandModule
{
    [RequireContext<CommandContext>(Services.RequiredContext.Guild)]
    [Command("ban")]
    public async Task Ban(UserId userId, TimeSpan deleteMessagesTime, [Remainder] string? reason = null)
    {
        await Context.Guild!.BanUserAsync(userId, new()
        {
            DeleteMessageSeconds = (int)deleteMessagesTime.TotalSeconds,
        }, new() { AuditLogReason = reason });

        ActionRowProperties actionRow = new(new List<ButtonProperties>
            {
                new ActionButtonProperties($"unban:{userId.Id}", ButtonStyle.Danger)
                {
                    Label = "Unban"
                }
            });
        MessageProperties message = new()
        {
            Content = Format.Bold($"{userId} got banned").ToString(),
            Components = new List<ComponentProperties>()
                {
                    actionRow
                },
            MessageReference = new(Context.Message),
            AllowedMentions = AllowedMentionsProperties.None,
        };
        await SendAsync(message);
    }

    [Command("unban")]
    public async Task Unban(UserId userId, [Remainder] string? reason = null)
    {
        await Context.Guild!.UnbanUserAsync(userId, new() { AuditLogReason = reason });
        await ReplyAsync(Format.Bold($"{userId} got unbanned").ToString());
    }
}