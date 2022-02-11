using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class NormalCommands : CommandModule
{
    [Command("say")]
    public Task Say([Remainder] string text)
    {
        return SendAsync(new MessageProperties() { Content = text, AllowedMentions = AllowedMentionsProperties.None });
    }

    [Command("reply")]
    public Task Reply([Remainder] string text)
    {
        return SendAsync(new MessageProperties() { Content = text, AllowedMentions = AllowedMentionsProperties.None, MessageReference = new(Context.Message) });
    }

    [Command("roles")]
    public async Task Roles(params DiscordId[] roles)
    {
        if (Context.User is GuildUser guildUser)
        {
            await guildUser.ModifyAsync(p => p.NewRolesIds = guildUser.RolesIds.Concat(roles).Distinct());
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
            var menu = CreateRolesMenu(Context.Guild!.Roles.Values, user.RolesIds);
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

    public static MenuProperties CreateRolesMenu(IEnumerable<GuildRole> guildRoles, IEnumerable<DiscordId> defaultValues)
    {
        var roles = guildRoles.Where(r => !r.Managed).OrderByDescending(r => r.Position).SkipLast(1);
        List<MenuSelectOptionProperties> options = new();
        foreach (var role in roles)
            options.Add(new(role.Name, role.Id.ToString()) { IsDefault = defaultValues.Contains(role.Id) });

        MenuProperties menu = new("roles")
        {
            Placeholder = "Select roles",
            MaxValues = roles.Count(),
            MinValues = 0,
            Options = options
        };

        return menu;
    }

    [Command("ping")]
    public Task Ping() => ReplyAsync($"Pong! {Context.Client.Latency} ms");

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
            Image = new(user.HasAvatar ? user.GetAvatarUrl(4096) : user.DefaultAvatarUrl),
            Color = new(0, 255, 0)
        };
        MessageProperties message = new()
        {
            Embeds = new List<EmbedProperties>()
            {
                embed
            },
            MessageReference = new(Context.Message, false),
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