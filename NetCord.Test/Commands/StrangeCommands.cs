using System.Runtime.InteropServices;

using NetCord.Commands;

namespace NetCord.Test;

public class StrangeCommands : CommandModule
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
        var channel = await UserHelper.GetDMChannelAsync(Context.Client, userId);
        await channel.SendMessageAsync(text);
    }

    [Command("delete", "remove")]
    public Task Delete() => Context.Message.DeleteAsync();

    [Command("delete", "remove", RequiredUserChannelPermissions = Permission.ManageMessages)]
    public Task Delete(DiscordId id)
    {
        return MessageHelper.DeleteAsync(Context.Client, Context.Channel, id);
    }

    [Command("react")]
    public Task React(string emoji)
    {
        ReactionEmoji reaction;
        if (System.Text.RegularExpressions.Regex.IsMatch(emoji, "(\u00a9|\u00ae|[\u2000-\u3300]|\ud83c[\ud000-\udfff]|\ud83d[\ud000-\udfff]|\ud83e[\ud000-\udfff])"))
        {
            reaction = new(emoji);
        }
        else
        {
            var span = emoji.AsSpan();
            var last = span.LastIndexOf(':');
            var first = span.IndexOf(':');
            var name = span[(last == first ? 1 : first + 1)..last];
            var id = span[(last + 1)..^1];
            reaction = new(name.ToString(), DiscordId.Parse(id.ToString()));
        }
        return Context.Message.AddReactionAsync(reaction);
    }

    [Command("button")]
    public Task Button()
    {
        MessageBuilder messageBuilder = new()
        {
            Content = "This is button:",
            Components = new(),
            MessageReference = new(Context.Message.Id),
            AllowedMentions = new()
            {
                ReplyMention = false
            }
        };
        ActionButton button = new("Click it!", "click it", MessageButtonStyle.Success)
        {
            EmojiId = DiscordId.Parse("888159212109197382")
        };
        ActionRow actionRow = new();
        actionRow.Buttons.Add(button);
        messageBuilder.Components.Add(actionRow);
        return SendAsync(messageBuilder.Build());
    }

    [Command("link")]
    public Task Link([Remainder] Uri url)
    {
        MessageBuilder message = new()
        {
            Components = new(),
            Content = "This is the message with the link",
            MessageReference = new(Context.Message),
            AllowedMentions = new()
            {
                ReplyMention = false
            },
            Embeds = new()
        };
        ActionRow actionRow = new();
        actionRow.Buttons.Add(new LinkButton("Link", url));
        message.Components.Add(actionRow);
        return SendAsync(message.Build());
    }

    [Command("s")]
    public Task S([DefaultParameterValue(null)] params string[] s)
    {
        if (s != null)
            return ReplyAsync("s: " + string.Join('\n', s));
        else
            return ReplyAsync("s: null");
    }

    [Command("dżejuś")]
    public async Task Dzejus()
    {
        MessageBuilder message = new()
        {
            Files = new(),
            MessageReference = new(Context.Message),
            AllowedMentions = new()
            {
                ReplyMention = false
            }
        };
        message.Files.Add(new("dżejuś.gif", "C:/Users/Kuba/Downloads/dżejuś.gif"));
        await SendAsync(message.Build());
    }

    [Command("exception", "e")]
    public static Task Exception()
    {
        throw new Exception("Exception was thrown");
    }

    [Command("wzium")]
    public Task Wzium([Remainder] Wzium? wzium = Test.Wzium.Wzium)
    {
        return ReplyAsync(wzium.ToString());
    }

    [Command("wziumy")]
    public Task Wziumy([AllowByValue] params Wzium[] wziumy)
    {
        return ReplyAsync(string.Join('\n', wziumy));
    }

    //[Command("wziumy")]
    //public Task Wziumy()
    //{
    //    return ReplyAsync("nie ma wziuma");
    //}

    [Command("messages")]
    public async Task Messages()
    {
        await foreach (var m in Context.Channel.GetMessagesAsync())
        {
            Console.WriteLine($"{m.Author.Username}: \t{m.Content} | {m.CreatedAt:g}");
        }
    }

    [Command("message")]
    public async Task Message(DiscordId id)
    {
        var m = await ChannelHelper.GetMessageAsync(Context.Client, Context.Channel, id);
        await ReplyAsync($"{m.Author}: {m.Content}");
    }

    [Command("id")]
    public Task Id([Remainder] UserId userId = null)
    {
        DiscordId id = userId != null ? userId.Id : Context.User;
        EmbedBuilder embed = new()
        {
            Title = $"Info about {id}",
            Fields = new()
        };
        embed.Fields.Add(new() { Title = "Id", Description = id.ToString() });
        embed.Fields.Add(new() { Title = "Created at", Description = new Timestamp(id.CreatedAt).ToString() });
        embed.Fields.Add(new() { Title = "Internal worker id", Description = id.InternalWorkerId.ToString() });
        embed.Fields.Add(new() { Title = "Internal process id", Description = id.InternalProcessId.ToString() });
        MessageBuilder message = new()
        {
            Embeds = new()
        };
        message.Embeds.Add(embed.Build());
        return SendAsync(message.Build());
    }

    [Command("menu")]
    public Task Menu(params string[] values)
    {
        values = values.Distinct().ToArray();
        MessageBuilder message = new()
        {
            Content = "Here is your menu:",
            Components = new(),
            MessageReference = new(Context.Message)
        };
        message.Components.Add(
            new Menu("menu")
            {
                Options = values.Select(v => new Menu.SelectOption(v, v)).ToList(),
                MaxValues = values.Length
            }
        );
        return SendAsync(message.Build());
    }

    [Command("timestamp")]
    public Task Timestamp([Remainder] DateTime time)
    {
        return ReplyAsync($"\\{new Timestamp(time)}");
    }

    [Command("bot-avatar")]
    public Task Avatar()
    {
        var newAvatar = Context.Message.Attachments.Values.FirstOrDefault();
        if (newAvatar == null)
            throw new Exception("Give an url or attachment");
        return Avatar(new(newAvatar.Url));
    }

    [Command("bot-avatar")]
    public async Task Avatar(Uri avatarUrl)
    {
        var a = await new HttpClient().GetByteArrayAsync(avatarUrl);
        await Context.Client.User.ModifyAsync(p => p.Avatar = new(new("image/png"), Convert.ToBase64String(a)));
    }

    [Command("spam")]
    public async Task Spam(int count)
    {
        SemaphoreSlim slim = new(5);
        var tasks = new Task[count];
        var names = Enum.GetValues<Wzium>().Select(w => w.ToString()).ToArray();
        for (int i = 0; i < count; i++)
        {
            tasks[i] = Task.Run(async () =>
            {
                await slim.WaitAsync();
                await SendAsync(names[new Random().Next(2)]);
                slim.Release();
            });
        }
        await Task.WhenAll(tasks);
        await ReplyAsync("Spammed!");
    }
    
    [Command("escape")]
    public Task Escape([Remainder] string text)
    {
        return ReplyAsync(Format.Escape(text).ToString());
    }
}

public enum Wzium
{
    Wzium,
    Wziumtek,
    Wziumastek,
}