using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands.Administrative;

[RequireContext<CommandContext>(Services.RequiredContext.Guild)]
[RequireUserPermissions<CommandContext>(Permissions.BanUsers), RequireBotPermissions<CommandContext>(Permissions.BanUsers)]
public class BanCommands : CommandModule<CommandContext>
{
    [RequireContext<CommandContext>(Services.RequiredContext.Guild)]
    [Command("ban")]
    public async Task Ban(UserId userId, TimeSpan deleteMessagesTime = default, [CommandParameter(Remainder = true)] string? reason = null)
    {
        await Context.Guild!.BanUserAsync(userId.Id, (int)deleteMessagesTime.TotalSeconds, new() { AuditLogReason = reason });

        ActionRowProperties actionRow = new(new ButtonProperties[]
            {
                new ActionButtonProperties($"unban:{userId.Id}", "Unban", ButtonStyle.Danger),
            });
        MessageProperties message = new()
        {
            Content = Format.Bold($"{userId} got banned").ToString(),
            Components = new List<ComponentProperties>()
                {
                    actionRow
                },
            MessageReference = new(Context.Message.Id),
            AllowedMentions = AllowedMentionsProperties.None,
        };
        await SendAsync(message);
    }

    [Command("unban")]
    public async Task Unban(UserId userId, [CommandParameter(Remainder = true)] string? reason = null)
    {
        await Context.Guild!.UnbanUserAsync(userId.Id, new() { AuditLogReason = reason });
        await ReplyAsync(Format.Bold($"{userId} got unbanned").ToString());
    }
}
