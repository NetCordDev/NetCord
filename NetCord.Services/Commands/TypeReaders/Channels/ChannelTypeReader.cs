using System.Globalization;

using NetCord.Gateway;

namespace NetCord.Services.Commands.TypeReaders;

public class ChannelTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            var channel = context.Message.Channel;
            if (channel is not null)
                return Task.FromResult<object?>(GetChannel<TextChannel>(channel, input.Span));
        }
        else
            return Task.FromResult<object?>(GetGuildChannel<Channel>(guild, input.Span));

        throw new EntityNotFoundException("The channel was not found.");
    }

    protected T GetChannel<T>(TextChannel channel, ReadOnlySpan<char> input)
    {
        if (MentionUtils.TryParseChannel(input, out var id))
        {
            if (id == channel.Id && channel is T t)
                return t;
        }

        if (channel is INamedChannel namedChannel)
        {
            if (input.SequenceEqual(namedChannel.Name) && channel is T t)
                return t;
        }

        if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
        {
            if (id == channel.Id && channel is T t)
                return t;
        }

        throw new EntityNotFoundException("The channel was not found.");
    }

    protected T GetGuildChannel<T>(Guild guild, ReadOnlySpan<char> input)
    {
        var channels = guild.Channels;
        var threads = guild.ActiveThreads;

        // by mention
        if (MentionUtils.TryParseChannel(input, out var id))
        {
            if (channels.TryGetValue(id, out var channel))
            {
                if (channel is T t)
                    return t;
            }
            else if (threads.TryGetValue(id, out var thread))
            {
                if (thread is T t)
                    return t;
            }
        }

        // by name
        if (input.Length <= 100)
        {
            using var enumerator = channels.Values.Concat((IEnumerable<INamedChannel>)threads.Values).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (input.SequenceEqual(current.Name) && current is T t)
                {
                    while (enumerator.MoveNext())
                    {
                        var current2 = enumerator.Current;
                        if (input.SequenceEqual(current2.Name) && current2 is T)
                            throw new InvalidOperationException("Too many channels found.");
                    }
                    return t;
                }
            }

            using var enumerator2 = threads.Values.GetEnumerator();
        }

        // by id
        if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
        {
            if (channels.TryGetValue(id, out var channel))
            {
                if (channel is T t)
                    return t;
            }
            else if (threads.TryGetValue(id, out var thread))
            {
                if (thread is T t)
                    return t;
            }
        }

        throw new EntityNotFoundException("The channel was not found.");
    }
}
