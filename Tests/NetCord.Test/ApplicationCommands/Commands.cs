using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace NetCord.Test.ApplicationCommands;

[SlashCommand("pn", "PN")]
[SlashCommand("permission-nested", "Permission")]
public class NestedCommand : ApplicationCommandModule<SlashCommandContext>
{
    [SubSlashCommand("a", "A")]
    [SubSlashCommand("add", "Permission add")]
    public Task AddAsync(int i, Permissions permission)
    {
        _ = i;
        return RespondAsync(InteractionCallback.Message(permission.ToString()));
    }

    [SubSlashCommand("remove", "Permission remove")]
    public static InteractionCallback Remove(int i, Permissions permission)
    {
        _ = i;
        return InteractionCallback.Message(permission.ToString());
    }

    [SubSlashCommand("l", "L")]
    [SubSlashCommand("list", "Permission list")]
    public class ListCommand : ApplicationCommandModule<SlashCommandContext>
    {
        [SubSlashCommand("u", "U")]
        [SubSlashCommand("user", "Permission list user")]
        public static InteractionCallback User(int i, Permissions permission)
        {
            _ = i;
            return InteractionCallback.Message(permission.ToString());
        }

        [SubSlashCommand("role", "Permission list role")]
        public Task RoleAsync(int i, Permissions permission)
        {
            _ = i;
            return RespondAsync(InteractionCallback.Message(permission.ToString()));
        }
    }
}

public class Commands : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("enum", "Enum!")]
    public static void Enum(ChannelFlags @enum)
    {
        _ = @enum;
        //return RespondAsync(InteractionCallback.Message(@enum.ToString()));
    }

    [SlashCommand("c", "C")]
    [SlashCommand("channel", "Channel")]
    public Task ChannelAsync(TextChannel channel)
    {
        return RespondAsync(InteractionCallback.Message(channel.ToString()));
    }

    [SlashCommand("test", "it's test", DefaultGuildUserPermissions = Permissions.AddReactions)]
    public Task TestAsync([SlashCommandParameter(MinValue = 10, MaxValue = 100)] int i1, int i2, int i3, int i4 = 4, int i5 = 5, int i6 = 6)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message($"{i1} {i2} {i3} {i4} {i5} {i6}"));
    }

    [SlashCommand("search", "Search using DuckDuckGo")]
    public Task SearchAsync([SlashCommandParameter(Description = "Search text", AutocompleteProviderType = typeof(DDGAutocomplete), MaxLength = 500)] string searchQuery)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message($"https://duckduckgo.com/?q={Uri.EscapeDataString(searchQuery)}"));
    }

    [SlashCommand("percentage", "Show formatted percentage")]
    public Task PercentageAsync([SlashCommandParameter(TypeReaderType = typeof(PercentageTypeReader))] byte percentage)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message($"{percentage}%"));
    }

    [RequireUserPermissions<SlashCommandContext>(Permissions.BanUsers), RequireBotPermissions<SlashCommandContext>(Permissions.BanUsers)]
    [SlashCommand("ban", "Bans a user")]
    public async Task BanAsync([SlashCommandParameter(Description = "User to ban")] User user, [SlashCommandParameter(Name = "delete_messages", Description = "Delete messages")] DeleteMessagesDays deleteMessages = DeleteMessagesDays.DontRemove, [MustContain("wzium")] string reason = "no reason")
    {
        if (Context.Guild is null)
            throw new InvalidOperationException("This command is available only in guild");

        await Context.Guild.BanUserAsync(user.Id, (int)deleteMessages, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = $"**{user} got banned**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [RequireUserPermissions<SlashCommandContext>(Permissions.ModerateUsers), RequireBotPermissions<SlashCommandContext>(Permissions.ModerateUsers)]
    [SlashCommand("mute", "Mutes a user")]
    public async Task MuteAsync([SlashCommandParameter(Description = "User to mute")] User user, double days, string reason = "no reason")
    {
        if (Context.Guild is null)
            throw new InvalidOperationException("This command is available only in guild");

        var until = DateTimeOffset.UtcNow.AddDays(days);
        await Context.Client.Rest.ModifyGuildUserAsync(Context.Guild.Id, user.Id, u => u.TimeOutUntil = until, new() { AuditLogReason = reason });
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = $"**{user} got muted until {new Timestamp(until, TimestampStyle.LongDateTime)}**", AllowedMentions = AllowedMentionsProperties.None }));
    }

    [SlashCommand("permissions", "Shows role permissions")]
    public Task PermissionsAsync(Role role)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(role.Permissions.ToString()));
    }

    //[SlashCommand("channel-name", "Shows channel name")]
    //public Task ChannelNameAsync(Channel? channel = null)
    //{
    //    return Context.Interaction.SendResponseAsync(InteractionCallback.ChannelMessageWithSource((channel ?? Context.Channel).ToString()));
    //}

    [SlashCommand("user", "Shows user info")]
    public Task UserAsync(User user)
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message(user.ToString()));
    }

    [SlashCommand("add-role", "Adds role to user or users")]
    public async Task AddRole(Mentionable mentionable, [SlashCommandParameter(Name = "role", Description = "Role to give")] Role roleToAdd)
    {
        if (mentionable.Type == MentionableType.Role)
        {
            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
            var roleId = mentionable.Role!.Id;
            foreach (var user in Context.Client.Cache.Guilds[Context.Guild!.Id].Users.Values.Where(u => u.RoleIds.Contains(roleId) && !u.RoleIds.Contains(roleToAdd.Id)))
                await user.AddRoleAsync(roleToAdd.Id);
            await Context.Interaction.ModifyResponseAsync(x =>
            {
                x.Content = $"Role {roleToAdd} was given to users with {mentionable.Role} role";
                x.AllowedMentions = AllowedMentionsProperties.None;
            });
        }
        else
        {
            await ((GuildUser)mentionable.User!).AddRoleAsync(roleToAdd.Id);
            await Context.Interaction.SendResponseAsync(InteractionCallback.Message(new() { Content = $"Role {roleToAdd} was given {mentionable.User}", AllowedMentions = AllowedMentionsProperties.None }));
        }
    }

    [SlashCommand("throw", "Throws exception")]
    public static Task ThrowAsync()
    {
        throw new();
    }

    [SlashCommand("dżejuś", "Shows dżejuś", GuildId = 856183259972763669)]
    public Task DzejusAsync()
    {
        return Context.Interaction.SendResponseAsync(InteractionCallback.Message("https://cdn.discordapp.com/attachments/927877869173084171/937493837335646238/dzejus.gif"));
    }

    [SlashCommand("permission", "Shows permission value")]
    public Task PermissionAsync(Permissions permission)
    {
        return RespondAsync(InteractionCallback.Message(((ulong)permission).ToString()));
    }

    ////[RequireContext<SlashCommandContext>(RequiredContext.Guild)]
    ////[RequireUserPermissions<SlashCommandContext>(default, Permission.ManageMessages), RequireBotPermission<SlashCommandContext>(default, Permission.ManageMessages)]
    //[SlashCommand("clear", "Clears channel")]
    //public async Task ClearAsync([MinValue(1)] int count, TextChannel? channel = null)
    //{
    //    if (channel is null)
    //    {
    //        if (!((GuildInteractionUser)Context.User).Permissions.HasFlag(Permission.ManageMessages))
    //            throw new("Missing permissions!");
    //        channel = Context.Channel;
    //    }
    //    else
    //    {
    //        if (!((IInteractionChannel)channel).Permissions.HasFlag(Permission.ManageMessages))
    //            throw new("Missing permissions!");
    //    }

    //    int i = 0;
    //    try
    //    {
    //        if (count > 100)
    //        {
    //            var first = await channel.GetMessagesAsync().Take(100).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => m.Id).ToListAsync();
    //            await RespondAsync(InteractionCallback.DeferredChannelMessageWithSource());
    //            var firstCount = first.Count;
    //            i = firstCount;
    //            if (firstCount > 0)
    //            {
    //                var lastId = first[^1];
    //                Task t = Context.Client.Rest.DeleteMessagesAsync(channel, first);
    //                if (firstCount == 100)
    //                {
    //                    var next = channel.GetMessagesBeforeAsync(lastId).Take(count - 100).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => { i++; return m.Id; });
    //                    await Context.Client.Rest.DeleteMessagesAsync(channel, next);
    //                }
    //                await t;
    //            }
    //        }
    //        else
    //        {
    //            var messages = await channel.GetMessagesAsync().Take(count).TakeWhile(m => m.CreatedAt >= DateTimeOffset.UtcNow.AddDays(-14)).Select(m => m.Id).ToListAsync();
    //            await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredChannelMessageWithSource());
    //            await Context.Client.Rest.DeleteMessagesAsync(channel, messages);
    //            i = messages.Count;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        await Context.Interaction.ModifyResponseAsync(m => m.Content = $"**{ex.Message}**");
    //        return;
    //    }
    //    await Context.Interaction.ModifyResponseAsync(m => m.Content = $"**Deleted {(i == 1 ? "1 message" : $"{i} messages")}**");
    //}

    [RequireNsfw<SlashCommandContext>()]
    [SlashCommand("nsfw", "You can use this command in nsfw channel"/*, Nsfw = true*/)]
    public static InteractionCallback Nsfw()
    {
        return InteractionCallback.Message("You used nsfw command!");
    }

    [SlashCommand("test2", "This is test")]
#pragma warning disable IDE0060 // Remove unused parameter
    public Task TestAsync(byte i1, decimal i2, double i3, Half i4, short i5, int i6, long i7, Int128 i8, nint i9, sbyte i10, float i11, ushort i12, uint i13, ulong i14, UInt128 i15, nuint i16)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        return RespondAsync(InteractionCallback.Message("Wzium"));
    }

    [SlashCommand("attachment", "You can pass attachment as a parameter!")]
    public async Task AttachmentAsync(Attachment attachment)
    {
        await RespondAsync(InteractionCallback.Message(new()
        {
            Content = "You sent it:",
            Attachments =
            [
                new(attachment.FileName, await new HttpClient().GetStreamAsync(attachment.Url))
            ]
        }));
    }

    [SlashCommand("guilduser", "Guild User")]
    public Task GuildUserAsync(GuildUser user)
    {
        return RespondAsync(InteractionCallback.Message(user.ToString()));
    }

    [SlashCommand("button", "Sends button")]
    public static InteractionCallback Button()
    {
        return InteractionCallback.Message(new()
        {
            Content = "Button:",
            Components =
            [
                new ActionRowProperties(
                [
                    new ButtonProperties("id", new EmojiProperties(1259973074573332480), ButtonStyle.Success),
                    new ButtonProperties("xd", new EmojiProperties("⭐"), ButtonStyle.Primary),
                    new LinkButtonProperties(new("https://google.com"), new EmojiProperties(942818016222138399)),
                ]),
            ],
        });
    }

    [SlashCommand("user-test", "Test")]
    public Task TestAsync(User user1, User user2)
    {
        return RespondAsync(InteractionCallback.Message($"{(user1 is GuildUser g ? (g.Nickname ?? g.Username) : user1.Username)}\n{(user2 is GuildUser g2 ? (g2.Nickname ?? g2.Username) : user2.Username)}"));
    }

    [SlashCommand("large-user-test", "Test")]
    public Task TestAsync(User user1, User user2, User user3, User user4, User user5, User user6, User user7, User user8, User user9)
    {
        return RespondAsync(InteractionCallback.Message($"{GetName(user1)}\n{GetName(user2)}\n{GetName(user3)}\n{GetName(user4)}\n{GetName(user5)}\n{GetName(user6)}\n{GetName(user7)}\n{GetName(user8)}\n{GetName(user9)}\n"));

        static string GetName(User u) => u is GuildUser g ? (g.Nickname ?? g.Username) : u.Username;
    }

    [SlashCommand("rate-limit-test", "test")]
    public async Task RateLimitTestAsync()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.Message("wz"));
        await Context.Interaction.ModifyResponseAsync(m => m.Content = "wz2");
        var message = await Context.Interaction.SendFollowupMessageAsync("xd");
        await Context.Interaction.ModifyFollowupMessageAsync(message.Id, m => m.Content = "xd2");
        await Context.Interaction.GetFollowupMessageAsync(message.Id);
        await Context.Interaction.GetResponseAsync();
        await Context.Interaction.DeleteFollowupMessageAsync(message.Id);
        await Context.Interaction.DeleteResponseAsync();
    }

    [SlashCommand("rate-limit-test2", "test")]
    public Task RateLimitTest2Async()
        => RateLimitTestAsync();

    [SlashCommand("entity-menus", "Entity Menus")]
    public Task EntityMenusAsync([SlashCommandParameter(Name = "min_values", MinValue = 0)] int minValues = 1, [SlashCommandParameter(Name = "max_values", MinValue = 2)] int maxValues = 2)
    {
        return RespondAsync(InteractionCallback.Message(new()
        {
            Components =
            [
                new UserMenuProperties("users")
                {
                    MinValues = minValues,
                    MaxValues = maxValues,
                    DefaultValues = [855528385677885470],
                },
                new RoleMenuProperties("roles")
                {
                    MinValues = minValues,
                    MaxValues = maxValues,
                    DefaultValues = [862347766324002827],
                },
                new MentionableMenuProperties("mentionables")
                {
                    MinValues = minValues,
                    MaxValues = maxValues,
                    DefaultValues =
                    [
                        new(803230269111926786, MentionableValueType.User),
                        new(913370324689633341, MentionableValueType.Role),
                    ],
                },
                new ChannelMenuProperties("channels")
                {
                    MinValues = minValues,
                    MaxValues = maxValues,
                    DefaultValues = [994276824584573038],
                },
            ]
        }));
    }
}

public enum DeleteMessagesDays
{
    [SlashCommandChoice("Don't remove")]
    DontRemove = 0 * 24 * 60 * 60,
    [SlashCommandChoice("Last 24 hours")]
    Last24Hours = 1 * 24 * 60 * 60,
    [SlashCommandChoice("Last 2 days")]
    Last2Days = 2 * 24 * 60 * 60,
    [SlashCommandChoice("Last 3 days")]
    Last3Days = 3 * 24 * 60 * 60,
    [SlashCommandChoice("Last 4 days")]
    Last4Days = 4 * 24 * 60 * 60,
    [SlashCommandChoice("Last 5 days")]
    Last5Days = 5 * 24 * 60 * 60,
    [SlashCommandChoice("Last 6 days")]
    Last6Days = 6 * 24 * 60 * 60,
    [SlashCommandChoice("Last week")]
    LastWeek = 7 * 24 * 60 * 60,
}
