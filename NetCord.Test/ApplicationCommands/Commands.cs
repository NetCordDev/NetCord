using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.SlashCommands;

public class Commands : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("test", "it's test")]
    public Task TestAsync([MinValue(10), MaxValue(100)] int i1, int i2, int i3, int i4 = 4, int i5 = 5, int i6 = 6)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"{i1} {i2} {i3} {i4} {i5} {i6}"));
    }

    [SlashCommand("search", "Search using DuckDuckGo")]
    public Task SearchAsync([Autocomplete(typeof(DDGAutocomplete))][SlashCommandParameter("Search text")] string query)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"https://duckduckgo.com/?q={Uri.EscapeDataString(query)}"));
    }

    [SlashCommand("percentage", "Show formatted percentage")]
    public Task PercentageAsync([TypeReader(typeof(PercentageTypeReader))] byte percentage)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource($"{percentage}%"));
    }

    [InteractionRequireUserChannelPermission<SlashCommandContext>(Permission.BanUsers), RequireBotPermission<SlashCommandContext>(Permission.BanUsers)]
    [SlashCommand("ban", "Bans a user")]
    public async Task BanAsync([SlashCommandParameter("user", "User to ban")] User user, [SlashCommandParameter("delete_messages", "Delete messages")] DeleteMessagesDays deleteMessages = DeleteMessagesDays.DontRemove, string reason = "no reason")
    {
        if (Context.Guild == null)
            throw new InvalidOperationException("This command is avaible only in guild");

        await Context.Guild.BanUserAsync(user, (int)deleteMessages, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**{user} got banned**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [InteractionRequireUserChannelPermission<SlashCommandContext>(Permission.ModerateUsers), RequireBotPermission<SlashCommandContext>(Permission.ModerateUsers)]
    [SlashCommand("mute", "Mutes a user")]
    public async Task MuteAsync([SlashCommandParameter("user", "User to mute")] User user, double days, string reason = "no reason")
    {
        if (Context.Guild == null)
            throw new InvalidOperationException("This command is avaible only in guild");

        var until = DateTimeOffset.UtcNow.AddDays(days);
        await Context.Client.Rest.ModifyGuildUserAsync(Context.Guild, user, u => u.TimeOutUntil = until, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"**{user} got muted until {new Timestamp(until, TimestampStyle.LongDateTime)}**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [SlashCommand("permissions", "Shows role permissions")]
    public Task PermissionsAsync(GuildRole role)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(role.Permissions.ToString()));
    }

    [SlashCommand("channel-name", "Shows channel name")]
    public Task ChannelNameAsync(Channel? channel = null)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource((channel ?? Context.Channel).ToString()));
    }

    [SlashCommand("user", "Shows user info")]
    public Task UserAsync(User user)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(user.ToString()));
    }

    [SlashCommand("add-role", "Adds role to user or users")]
    public async Task AddRole(Mentionable mentionable, [SlashCommandParameter("role", "Role to give")] GuildRole roleToAdd)
    {
        if (mentionable.Type == MentionableType.Role)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredChannelMessageWithSource());
            var roleId = mentionable.Role!.Id;
            foreach (var user in Context.Guild!.Users.Values.Where(u => u.RoleIds.Contains(roleId) && !u.RoleIds.Contains(roleToAdd)))
                await user.AddRoleAsync(roleToAdd);
            await Context.Interaction.ModifyResponseAsync(x =>
            {
                x.Content = $"Role {roleToAdd} was given to users with {mentionable.Role} role";
                x.AllowedMentions = AllowedMentionsProperties.None;
            });
        }
        else
        {
            await ((GuildUser)mentionable.User!).AddRoleAsync(roleToAdd);
            await Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource(new() { Content = $"Role {roleToAdd} was given {mentionable.User}", AllowedMentions = AllowedMentionsProperties.None }));
        }
    }

    [SlashCommand("throw", "Throws exception")]
    public static Task ThrowAsync()
    {
        throw new();
    }

    [SlashCommand("dżejuś", "Shows dżejuś", DefaultPermission = false, GuildId = 856183259972763669, AllowedUserIds = new ulong[] { 484036895391875093 })]
    public Task DzejusAsync()
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource("https://cdn.discordapp.com/attachments/927877869173084171/937493837335646238/dzejus.gif"));
    }

    [SlashCommand("permission", "Shows permission value")]
    public Task PermissionAsync(Permission permission)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(((ulong)permission).ToString()));
    }

    [RequireContext<SlashCommandContext>(RequiredContext.Guild)]
    [RequireUserPermission<SlashCommandContext>(default, Permission.ManageMessages), RequireBotPermission<SlashCommandContext>(default, Permission.ManageMessages)]
    [SlashCommand("clear", "Clears channel", GuildId = 856183259972763669)]
    public async Task ClearAsync([MinValue(1)] int count, TextChannel? channel = null)
    {
        channel ??= Context.Channel;
        int i = 0;
        try
        {
            if (count > 100)
            {
                var first = await channel.GetMessagesAsync().Take(100).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => m.Id).ToListAsync();
                await RespondAsync(InteractionCallback.DeferredChannelMessageWithSource());
                var firstCount = first.Count;
                i = firstCount;
                if (firstCount > 0)
                {
                    var lastId = first[^1];
                    Task t = Context.Client.Rest.DeleteMessagesAsync(channel, first);
                    if (firstCount == 100)
                    {
                        var next = channel.GetMessagesBeforeAsync(lastId).Take(count - 100).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => { i++; return m.Id; });
                        await Context.Client.Rest.DeleteMessagesAsync(channel, next);
                    }
                    await t;
                }
            }
            else
            {
                var messages = await channel.GetMessagesAsync().Take(count).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => m.Id).ToListAsync();
                await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredChannelMessageWithSource());
                await Context.Client.Rest.DeleteMessagesAsync(channel, messages);
                i = messages.Count;
            }
        }
        catch (Exception ex)
        {
            await Context.Interaction.ModifyResponseAsync(m => m.Content = $"**{ex.Message}**");
            return;
        }
        await Context.Interaction.ModifyResponseAsync(m => m.Content = $"**Deleted {(i == 1 ? "1 message" : $"{i} messages")}**");
    }

    [RequireNsfw<SlashCommandContext>]
    [SlashCommand("nsfw", "You can use this command in nsfw channel")]
    public Task NsfwAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource("You used nsfw command!"));
    }

    [SlashCommand("test2", "This is test")]
    public Task TestAsync(byte i1, decimal i2, double i3, Half i4, short i5, int i6, long i7, nint i8, sbyte i9, float i10, ushort i11, uint i12, ulong i13, nuint i14)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource("Wzium"));
    }

    [SlashCommand("attachment", "You can pass attachment as a parameter!")]
    public async Task AttachmentAsync(Attachment attachment)
    {
        await RespondAsync(InteractionCallback.ChannelMessageWithSource(new()
        {
            Content = "You sent it:",
            Attachments = new List<AttachmentProperties>
            {
                new(attachment.Filename, await new HttpClient().GetStreamAsync(attachment.Url))
            }
        }));
    }

    [SlashCommand("guilduser", "Guild User")]
    public Task GuildUserAsync(GuildUser user)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(user.ToString()));
    }

    [SlashCommand("button", "Send button")]
    public Task ButtonAsync()
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource(new()
        {
            Content = "Button:",
            Components = new List<ComponentProperties>
            {
                new ActionRowProperties(new List<ButtonProperties>
                {
                    new ActionButtonProperties("id", ButtonStyle.Success)
                    {
                        Emoji = new(942818016222138399)
                    },
                    new LinkButtonProperties(new("https://google.com"))
                    {
                        Emoji = new(942818016222138399)
                    }
                })
            }
        }));
    }

    [SlashCommand("user-test", "Test")]
    public Task TestAsync(User user1, User user2)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"{(user1 is GuildUser g ? (g.Nickname ?? g.Username) : user1.Username)}\n{(user2 is GuildUser g2 ? (g2.Nickname ?? g2.Username) : user2.Username)}"));
    }

    [SlashCommand("large-user-test", "Test")]
    public Task TestAsync(User user1, User user2, User user3, User user4, User user5, User user6, User user7, User user8, User user9)
    {
        return RespondAsync(InteractionCallback.ChannelMessageWithSource($"{GetName(user1)}\n{GetName(user2)}\n{GetName(user3)}\n{GetName(user4)}\n{GetName(user5)}\n{GetName(user6)}\n{GetName(user7)}\n{GetName(user8)}\n{GetName(user9)}\n"));

        static string GetName(User u) => u is GuildUser g ? (g.Nickname ?? g.Username) : u.Username;
    }
}

public enum DeleteMessagesDays
{
    [SlashCommandChoice("Don't remove")]
    DontRemove = 0,
    [SlashCommandChoice("Last 24 hours")]
    Last24Hours = 1,
    [SlashCommandChoice("Last 2 days")]
    Last2Days = 2,
    [SlashCommandChoice("Last 3 days")]
    Last3Days = 3,
    [SlashCommandChoice("Last 4 days")]
    Last4Days = 4,
    [SlashCommandChoice("Last 5 days")]
    Last5Days = 5,
    [SlashCommandChoice("Last 6 days")]
    Last6Days = 6,
    [SlashCommandChoice("Last week")]
    LastWeek = 7,
}