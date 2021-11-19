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

    [Command("delete", "remove")]
    public Task Delete() => Context.Message.DeleteAsync();

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
    public Task Wzium(Wzium? wzium = Test.Wzium.Wzium)
    {
        return ReplyAsync(wzium.ToString());
    }

    [Command("wziumy")]
    public Task Wziumy(params Wzium[] wziumy)
    {
        Wzium();
        return ReplyAsync(string.Join('\n', wziumy));
    }

    [Command("wziumy")]
    public Task Wziumy()
    {
        return ReplyAsync("nie ma wziuma");
    }

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
}

public enum Wzium
{
    Wzium,
    Wziumtek,
    Wziumastek,
}