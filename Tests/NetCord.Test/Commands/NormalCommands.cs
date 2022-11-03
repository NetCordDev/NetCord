using NetCord.Rest;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class NormalCommands : CommandModule<CommandContext>
{
    [Command("say")]
    public Task Say([Remainder] string text)
    {
        return SendAsync(new MessageProperties() { Content = text, AllowedMentions = AllowedMentionsProperties.None });
    }

    [Command("reply")]
    public Task Reply([Remainder] string text)
    {
        return SendAsync(new MessageProperties() { Content = text, AllowedMentions = AllowedMentionsProperties.None, MessageReference = new(Context.Message.Id) });
    }

    [Command("roles")]
    public async Task Roles(params ulong[] roles)
    {
        if (Context.User is GuildUser guildUser)
        {
            await guildUser.ModifyAsync(p => p.RoleIds = guildUser.RoleIds.Concat(roles).Distinct());
            await ReplyAsync("Added the roles!!!");
        }
        else
            await ReplyAsync("You are not in a guild");
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
                Components = new List<ComponentProperties>()
                {
                    menu
                }
            };
            await SendAsync(message);
        }
        else
            await ReplyAsync("Required context: Guild");
    }

    public static MenuProperties CreateRolesMenu(IEnumerable<GuildRole> guildRoles, IEnumerable<ulong> defaultValues)
    {
        var roles = guildRoles.Where(r => !r.Managed).OrderByDescending(r => r.Position).SkipLast(1);
        List<StringMenuSelectOptionProperties> options = new();
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
    public Task Ping() => ReplyAsync($"Pong! {Math.Round(Context.Client.Latency.GetValueOrDefault().TotalMilliseconds)} ms");

    [Command("pong")]
    public Task Pong() => ReplyAsync("Ping!");

    [Command("timer")]
    public Task Timer(TimeSpan time)
    {
        Timestamp s = new(DateTimeOffset.UtcNow + time);
        return ReplyAsync($"{s.ToString(TimestampStyle.RelativeTime)} ({s.ToString(TimestampStyle.LongTime)})");
    }

    [Command("user")]
    public async Task User([Remainder] GuildUser? user = null)
    {
        user ??= (GuildUser)Context.User;
        await ReplyAsync(user.Username);
    }

    [Command("avatar")]
    public Task Avatar([Remainder] GuildUser? user = null)
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
            Embeds = new List<EmbedProperties>()
            {
                embed
            },
            MessageReference = new(Context.Message.Id, false),
            AllowedMentions = new()
            {
                ReplyMention = false
            }
        };
        return SendAsync(message);
    }

    [Command("nick", "nickname")]
    public async Task Nickname(GuildUser user, [Remainder] string nickname)
    {
        await user.ModifyAsync(x => x.Nickname = nickname);
        await ReplyAsync(Format.Bold($"{user} updated!").ToString());
    }
}
