using System.Buffers;

using NetCord;
using NetCord.Gateway;
using NetCord.Rest;

namespace MentionTest;

[TestClass]
public class TryFormat
{
    private static readonly IReadOnlyList<ulong> _ids = [0, ulong.MaxValue, 1195682382234267720];

    private static readonly IReadOnlyList<SlashCommandMention> _slashCommandMentions = _ids.SelectMany<ulong, SlashCommandMention>(id => [new(id, "name"), new(id, "name", "name2"), new(id, "name", "name2", "name3")]).ToArray();

    private static readonly IReadOnlyList<Timestamp> _timestamps = _ids.SelectMany<ulong, Timestamp>(id =>
    {
        var timestamp = Snowflake.CreatedAt(id);
        return
        [
            new(timestamp),
            new(timestamp, TimestampStyle.ShortTime),
            new(timestamp, TimestampStyle.LongTime),
            new(timestamp, TimestampStyle.ShortDate),
            new(timestamp, TimestampStyle.LongDate),
            new(timestamp, TimestampStyle.ShortDateTime),
            new(timestamp, TimestampStyle.LongDateTime),
            new(timestamp, TimestampStyle.RelativeTime),
        ];
    }).ToArray();

    private static readonly IReadOnlyList<char> _timestampFormats = Enum.GetValues<TimestampStyle>().Select(s => (char)s).ToArray();

    private static readonly IReadOnlyList<ImageUrl> _imageUrls = _ids.Select(id => NetCord.ImageUrl.CustomEmoji(id, ImageFormat.Png)).ToArray();

    private static readonly IReadOnlyList<string> _imageUrlSizes = new[] { "16", "32", "64", "128", "256", "512", "1024", "2048", "4096" };

    private static readonly IReadOnlyList<GuildEmoji> _emojis = [
        .._ids
            .Select(id => new GuildEmoji(new()
            {
                Id = id,
                Name = "name",
            }, id, null!)),
        .._ids.Select(id => new GuildEmoji(new()
            {
                Id = id,
                Name = "name",
                Animated = true,
            }, id, null!))];

    private static readonly IReadOnlyList<LogMessage> _messages = new[]
    {
        NetCord.Gateway.LogMessage.Info("message"),
        NetCord.Gateway.LogMessage.Info("message", "description"),
        NetCord.Gateway.LogMessage.Error(new("message")),
    };

    private static readonly IReadOnlyList<CodeBlock> _codeBlocks = new CodeBlock[]
    {
        new("code"),
        new("code", "language"),
    };

    private static readonly IReadOnlyList<ApplicationCommand> _applicationCommands = _ids.Select(id => new ApplicationCommand(new()
    {
        Id = id,
        Name = "name",
    }, null!)).ToArray();

    private static readonly IReadOnlyList<ApplicationCommandOption> _applicationCommandOptions = _ids.Select(id => new ApplicationCommandOption(new()
    {
        Name = "name",
    }, "parentName", id)).ToArray();

    [TestMethod]
    public void User()
    {
        TestTryFormat(Mention.TryFormatUser, _ids, id => $"<@{id}>");
    }

    [TestMethod]
    public void Channel()
    {
        TestTryFormat(Mention.TryFormatChannel, _ids, id => $"<#{id}>");
    }

    [TestMethod]
    public void Role()
    {
        TestTryFormat(Mention.TryFormatRole, _ids, id => $"<@&{id}>");
    }

    [TestMethod]
    public void SlashCommand()
    {
        TestTryFormat(TryFormat, _slashCommandMentions, m => m.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, SlashCommandMention value) => value.TryFormat(destination, out charsWritten);
    }

    [TestMethod]
    public void Timestamp()
    {
        string? format = null;

        var timestamps = _timestamps;
        var timestampFormats = _timestampFormats;
        var timestampFormatsCount = timestampFormats.Count;

        int i = -1;
        while (true)
        {
            TestTryFormat(TryFormat, timestamps, t => t.ToString(format, null));

            if (++i == timestampFormatsCount)
                break;

            format = timestampFormats[i].ToString();
        }

        bool TryFormat(Span<char> destination, out int charsWritten, Timestamp value) => value.TryFormat(destination, out charsWritten, format);
    }

    [TestMethod]
    public void ImageUrl()
    {
        string? format = null;

        int i = -1;
        while (true)
        {
            TestTryFormat(TryFormat, _imageUrls, i => i.ToString(format, null));

            if (++i == _imageUrlSizes.Count)
                break;

            format = _imageUrlSizes[i];
        }

        bool TryFormat(Span<char> destination, out int charsWritten, ImageUrl value) => value.TryFormat(destination, out charsWritten, format);
    }

    [TestMethod]
    public void GuildEmoji()
    {
        TestTryFormat(TryFormat, _emojis, e => e.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, GuildEmoji value) => value.TryFormat(destination, out charsWritten);
    }

    [TestMethod]
    public void LogMessage()
    {
        TestTryFormat(TryFormat, _messages, m => m.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, LogMessage value) => value.TryFormat(destination, out charsWritten);
    }

    [TestMethod]
    public void CodeBlock()
    {
        TestTryFormat(TryFormat, _codeBlocks, c => c.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, CodeBlock value) => value.TryFormat(destination, out charsWritten);
    }

    [TestMethod]
    public void ApplicationCommand()
    {
        TestTryFormat(TryFormat, _applicationCommands, c => c.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, ApplicationCommand value) => value.TryFormat(destination, out charsWritten);
    }

    [TestMethod]
    public void ApplicationCommandOption()
    {
        TestTryFormat(TryFormat, _applicationCommandOptions, o => o.ToString());

        static bool TryFormat(Span<char> destination, out int charsWritten, ApplicationCommandOption value) => value.TryFormat(destination, out charsWritten);
    }

    private static void TestTryFormat<T>(TryFormatDelegate<T> tryFormat, IReadOnlyList<T> values, Func<T, string> getExpected)
    {
        int count = values.Count;
        var buffer = ArrayPool<char>.Shared.Rent(256);

        for (int i = 0; i < count; i++)
        {
            var value = values[i];
            var expected = getExpected(value);

            int bufferSize = 0;

            while (true)
            {
                Array.Clear(buffer);
                if (tryFormat(buffer.AsSpan(0, bufferSize), out var charsWritten, value))
                {
                    Assert.AreEqual(bufferSize, charsWritten);
                    Assert.AreEqual(expected, buffer.AsSpan(0, charsWritten).ToString());

                    Array.Clear(buffer);
                    Assert.IsTrue(tryFormat(buffer.AsSpan(0, bufferSize + 1), out charsWritten, value));
                    Assert.AreEqual(bufferSize, charsWritten);
                    Assert.AreEqual(expected, buffer.AsSpan(0, charsWritten).ToString());

                    break;
                }

                if (++bufferSize == buffer.Length)
                {
                    ArrayPool<char>.Shared.Return(buffer);
                    buffer = ArrayPool<char>.Shared.Rent(bufferSize * 2);
                }
            }
        }

        ArrayPool<char>.Shared.Return(buffer);
    }

    private delegate bool TryFormatDelegate<T>(Span<char> destination, out int charsWritten, T value);
}
