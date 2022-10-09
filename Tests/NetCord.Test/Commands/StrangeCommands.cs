using System.Runtime.InteropServices;

using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class StrangeCommands : CommandModule<CommandContext>
{
    [Command("say-dm", "saydm", "dm", "say-pv", "saypv", "pv")]
    public async Task SayDM([Remainder] string text)
    {
        var channel = await Context.User.GetDMChannelAsync();
        await channel.SendMessageAsync(text);
    }

    [Command("say-dm", "saydm", "dm", "say-pv", "saypv", "pv")]
    public async Task SayDM(UserId userId, [Remainder] string text)
    {
        var channel = await Context.Client.Rest.GetDMChannelAsync(userId);
        await channel.SendMessageAsync(text);
    }

    [Command("delete", "remove")]
    public Task Delete() => Context.Message.DeleteAsync();

    [RequireUserPermission<CommandContext>(default, Permission.ManageMessages), RequireBotPermission<CommandContext>(default, Permission.ManageMessages)]
    [Command("delete", "remove")]
    public Task Delete(Snowflake id)
    {
        return Context.Client.Rest.DeleteMessageAsync(Context.Message.ChannelId, id);
    }

    [Command("react")]
    public Task React(string emoji)
    {
        ReactionEmojiProperties reaction;
        if (System.Text.RegularExpressions.Regex.IsMatch(emoji, "(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])"))
            reaction = new(emoji);
        else
        {
            var span = emoji.AsSpan();
            var last = span.LastIndexOf(':');
            var first = span.IndexOf(':');
            var name = span[(last == first ? 1 : first + 1)..last];
            var id = span[(last + 1)..^1];
            reaction = new(name.ToString(), new(id.ToString()));
        }
        return Context.Message.AddReactionAsync(reaction);
    }

    [Command("button")]
    public Task Button()
    {
        ActionButtonProperties button = new("click it", "Click it!", new(888159212109197382), ButtonStyle.Success);
        ActionRowProperties actionRow = new(new ButtonProperties[] { button });
        MessageProperties messageBuilder = new()
        {
            Content = "This is button:",
            Components = new ComponentProperties[] { actionRow },
            MessageReference = new(Context.Message.Id),
            AllowedMentions = new()
            {
                ReplyMention = false
            },
            Embeds = new EmbedProperties[] { new() { Title = "Wzium" } },
        };
        return SendAsync(messageBuilder);
    }

    [Command("link")]
    public Task Link([Remainder] Uri url)
    {
        ActionRowProperties actionRow = new(new ButtonProperties[]
        {
            new LinkButtonProperties(url.AbsoluteUri, "Link"),
        });
        MessageProperties message = new()
        {
            Components = new ComponentProperties[]
            {
                actionRow
            },
            Content = "This is the message with the link",
            MessageReference = new(Context.Message),
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
        if (s != null)
            return ReplyAsync("s: " + string.Join('\n', s));
        else
            return ReplyAsync("s: null");
    }

    [Command("dżejuś", "dzejus", "jjay31")]
    public Task Dzejus()
    {
        AttachmentProperties file = new("dżejuś.gif", "C:/Users/Kuba/Downloads/dżejuś.gif") { Description = "Dżejuś" };
        MessageProperties message = new()
        {
            Attachments = new AttachmentProperties[] { file },
            MessageReference = new(Context.Message),
            AllowedMentions = AllowedMentionsProperties.None
        };
        return SendAsync(message);
    }

    [Command("exception", "e")]
    public static Task Exception()
    {
        throw new Exception("Exception was thrown");
    }

    [Command("wzium")]
    public Task Wzium([Remainder] Wzium wzium = Commands.Wzium.Wzium)
    {
        return ReplyAsync(wzium.ToString());
    }

    [Command("wziumy")]
    public Task Wziumy([AllowByValue] params Wzium[] wziumy)
    {
        return ReplyAsync(string.Join('\n', wziumy));
    }

    [Command("messages")]
    public async Task Messages(Snowflake? channelId = null)
    {
        channelId ??= Context.Message.ChannelId;
        await foreach (var m in Context.Client.Rest.GetMessagesAsync(channelId.GetValueOrDefault()))
            Console.WriteLine($"{m.Author.Username}: \t{m.Content} | {m.CreatedAt:g}");
    }

    [Command("message")]
    public async Task Message(Snowflake id)
    {
        var m = await Context.Client.Rest.GetMessageAsync(Context.Message.ChannelId, id);
        await ReplyAsync($"{m.Author}: {m.Content}");
    }

    [Command("id")]
    public Task Id([Remainder] UserId? userId = null)
    {
        var id = userId != null ? userId.Id : Context.User;
        List<EmbedFieldProperties> fields = new();
        EmbedProperties embed = new()
        {
            Title = $"Info about {id}",
            Fields = fields
        };
        fields.Add(new() { Title = "Id", Description = id.ToString()! });
        fields.Add(new() { Title = "Created at", Description = new Timestamp(id.CreatedAt).ToString() });
        fields.Add(new() { Title = "Internal worker id", Description = id.InternalWorkerId.ToString() });
        fields.Add(new() { Title = "Internal process id", Description = id.InternalProcessId.ToString() });
        MessageProperties message = new()
        {
            Embeds = new EmbedProperties[] { embed }
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
            Components = new ComponentProperties[] { new MenuProperties("menu", values.Select(v => new MenuSelectOptionProperties(v, v))) { MaxValues = values.Length } },
            MessageReference = new(Context.Message)
        };
        return SendAsync(message);
    }

    [Command("timestamp")]
    public Task Timestamp([Remainder] DateTime time)
    {
        return ReplyAsync($"\\{new Timestamp(time)}");
    }

    [Command("bot-avatar")]
    public Task BotAvatar()
    {
        var newAvatar = Context.Message.Attachments.Values.FirstOrDefault();
        if (newAvatar == null)
            throw new Exception("Give an url or attachment");
        return BotAvatar(new(newAvatar.Url));
    }

    [Command("bot-avatar")]
    public async Task BotAvatar(Uri avatarUrl)
    {
        var a = await new HttpClient().GetByteArrayAsync(avatarUrl);
        await Context.Client.User!.ModifyAsync(p => p.Avatar = new ImageProperties(a, ImageFormat.Png));
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
                await SendAsync(names[new Random().Next(2)]);
                slim.Release();
            });
        await Task.WhenAll(tasks);
        await ReplyAsync("Spammed!");
    }

    [Command("escape")]
    public Task Escape([Remainder] string text)
    {
        return ReplyAsync(Format.Escape(text).ToString());
    }

    [Command("quote", Priority = 1)]
    public async Task Quote(Snowflake messageId)
        => await ReplyAsync(Format.Quote((await Context.Client.Rest.GetMessageAsync(Context.Message.ChannelId, messageId)).Content));

    [Command("quote", Priority = 0)]
    public Task Quote(string text) => ReplyAsync(Format.Quote(text).ToString());

    [Command("codeblock")]
    public Task CodeBlock([Remainder] CodeBlock codeBlock)
    {
        return ReplyAsync(codeBlock.ToString());
    }

    [Command("embed")]
    public Task Embed()
    {
        EmbedProperties embedBuilder = new()
        {
            Fields = new EmbedFieldProperties[] { new() { Title = "xd", Description = "wzium" } }
        };
        return SendAsync(new MessageProperties() { Embeds = new EmbedProperties[] { embedBuilder } });
    }

    [Command("reverse")]
    public Task Reverse([Remainder][TypeReader(typeof(ReverseStringTypeReader))] string s)
    {
        return ReplyAsync(s);
    }

    [Command("attachment")]
    public Task AttachmentAsync()
    {
        AttachmentProperties attachment = new("dzejus.gif", "C:/Users/Kuba/Downloads/dżejuś.gif") { Description = "Dżejuś" };
        return SendAsync(new()
        {
            Attachments = new AttachmentProperties[] { attachment },
            Embeds = new EmbedProperties[] { new() { Image = attachment } }
        });
    }
}

public enum Wzium
{
    Wzium,
    Wziumtek,
    Wziumastek,
}
