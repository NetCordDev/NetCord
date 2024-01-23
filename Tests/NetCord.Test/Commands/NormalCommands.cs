using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class NormalCommands : CommandModule<CommandContext>
{
    private readonly string? _wzium;

    public NormalCommands(string wzium)
    {
        _wzium = wzium;
    }

    public NormalCommands()
    {
        _wzium = "12";
    }

    [Command("channel")]
    public Task ChannelAsync(GroupDMChannel channel)
    {
        return ReplyAsync(channel.ToString());
    }

    [Command("say")]
    public Task Say([CommandParameter(Remainder = true)] ReadOnlyMemory<char> text)
    {
        return SendAsync(new MessageProperties() { Content = text.ToString(), AllowedMentions = AllowedMentionsProperties.None });
    }

    [Command("reply")]
    public Task Reply([CommandParameter(Remainder = true)] string text)
    {
        return SendAsync(new MessageProperties() { Content = text, AllowedMentions = AllowedMentionsProperties.None, MessageReference = new(Context.Message.Id) });
    }

    [Command("roles")]
    public async Task<ReplyMessageProperties> Roles(params ulong[] roles)
    {
        if (Context.User is GuildUser guildUser)
        {
            await guildUser.ModifyAsync(p => p.RoleIds = guildUser.RoleIds.Concat(roles).Distinct());
            return "Added the roles!!!";
        }
        else
            return "You are not in a guild";
    }

    [Command("roles")]
    public async Task Roles()
    {
        if (Context.User is GuildUser user)
        {
            var menu = CreateRolesMenu(Context.Guild!.Roles.Values, user.RoleIds);
            MessageProperties message = new()
            {
                Content = "Select roles",
                Components = new ComponentProperties[] { menu },
            };
            await SendAsync(message);
        }
        else
            await ReplyAsync("Required context: Guild");
    }

    public static MenuProperties CreateRolesMenu(IEnumerable<Role> guildRoles, IEnumerable<ulong> defaultValues)
    {
        var roles = guildRoles.Where(r => !r.Managed).OrderByDescending(r => r.Position).SkipLast(1);
        List<StringMenuSelectOptionProperties> options = [];
        foreach (var role in roles)
            options.Add(new(role.Name, role.Id.ToString()!) { Default = defaultValues.Contains(role.Id) });

        StringMenuProperties menu = new("roles", options)
        {
            Placeholder = "Select roles",
            MaxValues = options.Count,
            MinValues = 0,
        };

        return menu;
    }

    [Command("ping")]
    public Task Ping() => ReplyAsync($"Pong! {Math.Round(Context.Client.Latency.TotalMilliseconds)} ms");

    [Command("pong")]
    public Task Pong() => ReplyAsync("Ping!");

    [Command("timer")]
    public Task Timer(TimeSpan time)
    {
        Timestamp s = new(DateTimeOffset.UtcNow + time);
        return ReplyAsync($"{s.ToString(TimestampStyle.RelativeTime)} ({s.ToString(TimestampStyle.LongTime)})");
    }

    [Command("user")]
    public Task User([CommandParameter(Remainder = true)] UserId? user = null)
    {
        user ??= new(Context.User.Id, Context.User);
        return ReplyAsync(user.ToString());
    }

    [Command("avatar")]
    public Task Avatar([CommandParameter(Remainder = true)] GuildUser? user = null)
    {
        user ??= (GuildUser)Context.User;
        EmbedProperties embed = new()
        {
            Title = $"Avatar of {user.Username}#{user.Discriminator}",
            Image = new(user.HasAvatar ? user.GetAvatarUrl().ToString(4096) : user.DefaultAvatarUrl.ToString()),
            Color = new(0, 255, 0)
        };
        MessageProperties message = new()
        {
            Embeds =
            [
                embed
            ],
            MessageReference = new(Context.Message.Id, false),
            AllowedMentions = new()
            {
                ReplyMention = false
            }
        };
        return SendAsync(message);
    }

    [Command("nick", "nickname")]
    public async Task Nickname(GuildUser user, [CommandParameter(Remainder = true)] string nickname)
    {
        await user.ModifyAsync(x => x.Nickname = nickname);
        await ReplyAsync(Format.Bold($"{user} updated!").ToString());
    }
}
