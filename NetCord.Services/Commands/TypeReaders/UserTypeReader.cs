using System.Globalization;

using NetCord.Gateway;

namespace NetCord.Services.Commands.TypeReaders;

public class UserTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            if (context.Message.Channel is DMChannel dMchannel)
                return new(GetUser(dMchannel, input.Span));
        }
        else
            return new(GetGuildUser(guild, input.Span));

        return new(TypeReaderResult.Fail("The user was not found."));
    }

    protected TypeReaderResult GetUser(DMChannel dMChannel, ReadOnlySpan<char> input)
    {
        var users = dMChannel.Users;

        // by mention
        if (Mention.TryParseUser(input, out var id))
        {
            if (users.TryGetValue(id, out var user))
                return TypeReaderResult.Success(user);
        }

        // by name and tag
        if (input.Length is >= 7 and <= 37 && input[^5] == '#')
        {
            if (ushort.TryParse(input[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
            {
                var username = input[..^5];
                foreach (var user in users.Values)
                {
                    if (user.Discriminator == discriminator && username.SequenceEqual(user.Username))
                        return TypeReaderResult.Success(user);
                }
            }
        }

        // by name
        if (input.Length is <= 32 and >= 2)
        {
            using var enumerator = users.Values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (input.SequenceEqual(current.Username))
                {
                    while (enumerator.MoveNext())
                    {
                        var current2 = enumerator.Current;
                        if (input.SequenceEqual(current2.Username))
                            return TypeReaderResult.Fail("Too many users found.");
                    }
                    return TypeReaderResult.Success(current);
                }
            }
        }

        // by id
        if (Snowflake.TryParse(input, out id))
        {
            if (users.TryGetValue(id, out var user))
                return TypeReaderResult.Success(user);
        }

        return TypeReaderResult.Fail("The user was not found.");
    }

    protected TypeReaderResult GetGuildUser(Guild guild, ReadOnlySpan<char> input)
    {
        var users = guild.Users;

        // by mention
        if (Mention.TryParseUser(input, out var id))
        {
            if (users.TryGetValue(id, out var user))
                return TypeReaderResult.Success(user);
        }

        var len = input.Length;

        // by name and tag
        if (len is >= 7 and <= 37 && input[^5] == '#')
        {
            if (ushort.TryParse(input[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
            {
                var username = input[..^5];
                foreach (var user in users.Values)
                {
                    if (user.Discriminator == discriminator && username.SequenceEqual(user.Username))
                        return TypeReaderResult.Success(user);
                }
            }
        }

        // by name or nickname
        if (len <= 32)
        {
            using var enumerator = users.Values.GetEnumerator();
            if (len >= 2)
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (input.SequenceEqual(current.Username) || input.SequenceEqual(current.Nickname))
                    {
                        while (enumerator.MoveNext())
                        {
                            var current2 = enumerator.Current;
                            if (input.SequenceEqual(current2.Username) || input.SequenceEqual(current2.Nickname))
                                return TypeReaderResult.Fail("Too many users found.");
                        }
                        return TypeReaderResult.Success(current);
                    }
                }
            }
            else
            {
                while (enumerator.MoveNext())
                {
                    var current = enumerator.Current;
                    if (input.SequenceEqual(current.Nickname))
                    {
                        while (enumerator.MoveNext())
                        {
                            var current2 = enumerator.Current;
                            if (input.SequenceEqual(current2.Nickname))
                                return TypeReaderResult.Fail("Too many users found.");
                        }
                        return TypeReaderResult.Success(current);
                    }
                }
            }
        }

        // by id
        if (Snowflake.TryParse(input, out id))
        {
            if (users.TryGetValue(id, out var user))
                return TypeReaderResult.Success(user);
        }

        return TypeReaderResult.Fail("The user was not found.");
    }
}
