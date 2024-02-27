using System.Runtime.InteropServices;

using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public partial class StrangeCommands : CommandModule<CommandContext>
{
    [Command("x")]
    public Task TestXAsync(nint wsz = default)
    {
        return ReplyAsync(wsz.ToString());
    }

    [Command("say-dm", "saydm", "dm", "say-pv", "saypv", "pv")]
    public async Task SayDM([CommandParameter(Remainder = true)] string text)
    {
        var channel = await Context.User.GetDMChannelAsync();
        await channel.SendMessageAsync(text);
    }

    [Command("say-dm", "saydm", "dm", "say-pv", "saypv", "pv")]
    public async Task SayDM(UserId userId, [CommandParameter(Remainder = true)] string text)
    {
        var channel = await Context.Client.Rest.GetDMChannelAsync(userId.Id);
        await channel.SendMessageAsync(text);
    }

    [Command("delete", "remove")]
    public Task Delete() => Context.Message.DeleteAsync();

    [RequireUserPermissions<CommandContext>(default, Permissions.ManageMessages), RequireBotPermissions<CommandContext>(default, Permissions.ManageMessages)]
    [Command("delete", "remove")]
    public Task Delete(ulong id)
    {
        return Context.Client.Rest.DeleteMessageAsync(Context.Message.ChannelId, id);
    }

    [Command("react")]
    public Task React(string emoji)
    {
        ReactionEmojiProperties reaction;
        if (EmojiRegex().IsMatch(emoji))
            reaction = new(emoji);
        else
        {
            var span = emoji.AsSpan();
            var last = span.LastIndexOf(':');
            var first = span.IndexOf(':');
            var name = span[(last == first ? 1 : first + 1)..last];
            var id = span[(last + 1)..^1];
            reaction = new(name.ToString(), Snowflake.Parse(id));
        }
        return Context.Message.AddReactionAsync(reaction);
    }

    [Command("button")]
    public Task Button()
    {
        ButtonProperties button = new("click it", "Click it!", new(888159212109197382), ButtonStyle.Success);
        ActionRowProperties actionRow = new([button]);
        MessageProperties messageBuilder = new()
        {
            Content = "This is button:",
            Components = [actionRow],
            MessageReference = new(Context.Message.Id),
            AllowedMentions = new()
            {
                ReplyMention = false
            },
            Embeds = [new() { Title = "Wzium" }],
        };
        return SendAsync(messageBuilder);
    }

    [Command("link")]
    public Task Link([CommandParameter(Remainder = true)] Uri url)
    {
        ActionRowProperties actionRow = new(
        [
            new LinkButtonProperties(url.AbsoluteUri, "Link"),
        ]);
        MessageProperties message = new()
        {
            Components =
            [
                actionRow
            ],
            Content = "This is the message with the link",
            MessageReference = new(Context.Message.Id),
            AllowedMentions = new()
            {
                ReplyMention = false
            },
        };
        return SendAsync(message);
    }

    [Command("s")]
    public Task S([DefaultParameterValue(null)] params string[]? s)
    {
        if (s is not null)
            return ReplyAsync("s: " + string.Join('\n', s));
        else
            return ReplyAsync("s: null");
    }

    [Command("dżejuś", "dzejus", "jjay31")]
    public Task Dzejus()
    {
        AttachmentProperties file = new("dżejuś.gif", File.OpenRead("C:/Users/Kuba/Downloads/dżejuś.gif")) { Description = "Dżejuś" };
        MessageProperties message = new()
        {
            Attachments = [file],
            MessageReference = new(Context.Message.Id),
            AllowedMentions = AllowedMentionsProperties.None
        };
        return SendAsync(message);
    }

    [Command("exception", "e")]
    public static Task Exception()
    {
        throw new("Exception was thrown");
    }

    [Command("wzium")]
    public Task Wzium([CommandParameter(Remainder = true)] Wzium wzium = Commands.Wzium.Wzium)
    {
        return ReplyAsync(wzium.ToString());
    }

    [Command("wziumy")]
    public Task Wziumy([AllowByValue] params Wzium[] wziumy)
    {
        return ReplyAsync(string.Join('\n', wziumy));
    }

    [Command("messages")]
    public async Task Messages(ulong? channelId = null)
    {
        channelId ??= Context.Message.ChannelId;
        await foreach (var m in Context.Client.Rest.GetMessagesAsync(channelId.GetValueOrDefault()))
            Console.WriteLine($"{m.Author.Username}: \t{m.Content} | {m.CreatedAt:g}");
    }

    [Command("message")]
    public async Task Message(ulong id)
    {
        var m = await Context.Client.Rest.GetMessageAsync(Context.Message.ChannelId, id);
        await ReplyAsync($"{m.Author}: {m.Content}");
    }

    [Command("id")]
    public Task Id([CommandParameter(Remainder = true)] UserId? userId = null)
    {
        var id = userId is not null ? userId.Id : Context.User.Id;
        var fields = new EmbedFieldProperties[]
        {
            new() { Name = "Id", Value = id.ToString()! },
            new() { Name = "Created At", Value = new Timestamp(Snowflake.CreatedAt(id)).ToString() },
            new() { Name = "Internal Worker Id", Value = Snowflake.InternalWorkerId(id).ToString() },
            new() { Name = "Internal Process Id", Value = Snowflake.InternalProcessId(id).ToString() },
            new() { Name = "Increment", Value = Snowflake.Increment(id).ToString() },
        };
        EmbedProperties embed = new()
        {
            Title = $"Info about {id}",
            Fields = fields
        };
        MessageProperties message = new()
        {
            Embeds = [embed]
        };
        return SendAsync(message);
    }

    [Command("menu")]
    public Task Menu(params string[] values)
    {
        values = values.Distinct().ToArray();
        MessageProperties message = new()
        {
            Content = "Here is your menu:",
            Components = [new StringMenuProperties("menu", values.Select(v => new StringMenuSelectOptionProperties(v, v))) { MaxValues = values.Length }],
            MessageReference = new(Context.Message.Id)
        };
        return SendAsync(message);
    }

    [Command("timestamp")]
    public Task Timestamp([CommandParameter(Remainder = true)] DateTime time)
    {
        return ReplyAsync($"\\{new Timestamp(time)}");
    }

    [Command("bot-avatar")]
    public Task BotAvatar()
    {
        var newAvatar = Context.Message.Attachments.Values.FirstOrDefault();
        return newAvatar is null ? throw new("Give an url or attachment") : BotAvatar(new(newAvatar.Url));
    }

    [Command("bot-avatar")]
    public async Task BotAvatar(Uri avatarUrl)
    {
        var a = await new HttpClient().GetByteArrayAsync(avatarUrl);
        await Context.Client.Cache.User!.ModifyAsync(p => p.Avatar = new ImageProperties(ImageFormat.Png, a));
    }

    [Command("spam")]
    public async Task Spam(int count)
    {
        SemaphoreSlim slim = new(5);
        var tasks = new Task[count];
        var names = Enum.GetValues<Wzium>().Select(w => w.ToString()).ToArray();
        for (var i = 0; i < count; i++)
            tasks[i] = Task.Run(async () =>
            {
                await slim.WaitAsync();
                await SendAsync(names[Random.Shared.Next(2)]);
                slim.Release();
            });
        await Task.WhenAll(tasks);
        await ReplyAsync("Spammed!");
    }

    [Command("escape")]
    public Task Escape([CommandParameter(Remainder = true)] string text)
    {
        return ReplyAsync(Format.Escape(text).ToString());
    }

    [Command("quote", Priority = 1)]
    public async Task Quote(ulong messageId)
        => await ReplyAsync(Format.Quote((await Context.Client.Rest.GetMessageAsync(Context.Message.ChannelId, messageId)).Content));

    [Command("quote", Priority = 0)]
    public Task Quote(string text) => ReplyAsync(Format.Quote(text).ToString());

    [Command("codeblock")]
    public Task CodeBlock([CommandParameter(Remainder = true)] CodeBlock codeBlock)
    {
        return ReplyAsync(codeBlock.ToString());
    }

    [Command("embed")]
    public Task Embed()
    {
        EmbedProperties embedBuilder = new()
        {
            Fields = [new() { Name = "xd", Value = "wzium" }]
        };
        return SendAsync(new MessageProperties() { Embeds = [embedBuilder] });
    }

    [Command("reverse")]
    public Task Reverse([CommandParameter(Remainder = true, TypeReaderType = typeof(ReverseStringTypeReader))] string s)
    {
        return ReplyAsync(s);
    }

    [Command("attachment")]
    public Task AttachmentAsync()
    {
        AttachmentProperties attachment = new("dzejus.gif", File.OpenRead("C:/Users/Kuba/Downloads/dżejuś.gif")) { Description = "Dżejuś" };
        return SendAsync(new()
        {
            Attachments = [attachment],
            Embeds = [new() { Image = attachment }]
        });
    }

    [Command("static")]
    public static Task StaticAsync()
    {
        Console.WriteLine("Used static command!");
        return Task.CompletedTask;
    }

    [Command("static")]
    public static Task StaticAsync(string s)
    {
        Console.WriteLine($"Used static command with {nameof(s)}: {s}");
        return Task.CompletedTask;
    }

    [Command("static")]
    public static Task StaticAsync(string x, params string[] s)
    {
        Console.WriteLine($"Used static command with {nameof(x)}: {x} and {nameof(s)}: {string.Join(", ", s)}");
        return Task.CompletedTask;
    }

    [Command("button")]
    public Task ButtonAsync(string customId)
    {
        return SendAsync(new()
        {
            Components =
            [
                new ActionRowProperties(
                [
                    new ButtonProperties(customId, "Button", ButtonStyle.Success),
                ]),
            ],
        });
    }

    [System.Text.RegularExpressions.GeneratedRegex("(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])")]
    private static partial System.Text.RegularExpressions.Regex EmojiRegex();
}

public enum Wzium
{
    Wzium,
    Wziumtek,
    Wziumastek,
}
