using System.Globalization;

namespace NetCord.Services.Commands.TypeReaders;

public class UserIdTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Message.Guild;
        if (guild is null)
        {
            if (context.Message.Channel is DMChannel dm)
            {
                var users = dm.Users;
                var span = input.Span;

                // by mention
                if (MentionUtils.TryParseUser(span, out var id))
                    return Task.FromResult<object?>(new UserId(id, users.GetValueOrDefault(id)));

                // by name and tag
                if (span.Length is >= 7 and <= 37 && span[^5] == '#')
                {
                    if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                    {
                        var username = span[..^5];
                        foreach (var user in users.Values)
                        {
                            if (user.Discriminator == discriminator && username.SequenceEqual(user.Username))
                                return Task.FromResult<object?>(new UserId(user.Id, user));
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
                        if (span.SequenceEqual(current.Username))
                        {
                            while (enumerator.MoveNext())
                            {
                                var current2 = enumerator.Current;
                                if (span.SequenceEqual(current2.Username))
                                    goto TooManyFound;
                            }
                            return Task.FromResult<object?>(new UserId(current.Id, current));
                        }
                    }
                }

                // by id
                if (ulong.TryParse(span, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                    return Task.FromResult<object?>(new UserId(id, users.GetValueOrDefault(id)));
            }
        }
        else
        {
            var users = guild.Users;
            var span = input.Span;

            // by mention
            if (MentionUtils.TryParseUser(span, out var id))
                return Task.FromResult<object?>(new UserId(id, users.GetValueOrDefault(id)));

            var len = span.Length;

            // by name and tag
            if (len is >= 7 and <= 37 && span[^5] == '#')
            {
                if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                {
                    var username = span[..^5];
                    foreach (var user in users.Values)
                    {
                        if (user.Discriminator == discriminator && username.SequenceEqual(user.Username))
                            return Task.FromResult<object?>(new UserId(user.Id, user));
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
                        if (span.SequenceEqual(current.Username) || span.SequenceEqual(current.Nickname))
                        {
                            while (enumerator.MoveNext())
                            {
                                var current2 = enumerator.Current;
                                if (span.SequenceEqual(current2.Username) || span.SequenceEqual(current2.Nickname))
                                    goto TooManyFound;
                            }
                            return Task.FromResult<object?>(new UserId(current.Id, current));
                        }
                    }
                }
                else
                {
                    while (enumerator.MoveNext())
                    {
                        var current = enumerator.Current;
                        if (span.SequenceEqual(current.Nickname))
                        {
                            while (enumerator.MoveNext())
                            {
                                var current2 = enumerator.Current;
                                if (span.SequenceEqual(current2.Nickname))
                                    goto TooManyFound;
                            }
                            return Task.FromResult<object?>(new UserId(current.Id, current));
                        }
                    }
                }
            }

            // by id
            if (ulong.TryParse(span, NumberStyles.None, CultureInfo.InvariantCulture, out id))
                return Task.FromResult<object?>(new UserId(id, users.GetValueOrDefault(id)));
        }

        throw new EntityNotFoundException("The user was not found.");

        TooManyFound:
        throw new InvalidOperationException("Too many users found.");
    }
}
