using System.Reflection;

namespace NetCord.Services.Commands.TypeReaders;

public class UserIdTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options)
    {
        var channel = context.Message.Channel;
        var guild = context.Message.Guild;
        if (guild != null)
        {
            IReadOnlyDictionary<Snowflake, GuildUser> users = guild.Users;
            var span = input.Span;
            var s = span.ToString();

            // by id
            if (Snowflake.TryCreate(s, out Snowflake id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }


            // by mention
            if (MentionUtils.TryParseUser(span, out id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], out var discriminator))
                {
                    GuildUser? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
            // by name or nickname
            else
            {
                var len = input.Length;
                if (len <= 32)
                {
                    GuildUser? user;
                    try
                    {
                        user = users.Values.SingleOrDefault(len >= 2 ? u => u.Username == s || u.Nickname == s : u => u.Nickname == s);
                    }
                    catch
                    {
                        throw new AmbiguousMatchException("Too many users found.");
                    }
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
        }
        else if (context.Message.Channel is DMChannel dm)
        {
            IReadOnlyDictionary<Snowflake, User> users = dm.Users;
            var span = input.Span;
            var s = span.ToString();

            // by id
            if (Snowflake.TryCreate(s, out Snowflake id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }


            // by mention
            if (MentionUtils.TryParseUser(span, out id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], out var discriminator))
                {
                    User? user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
            // by name
            else
            {
                if (input.Length is <= 32 and >= 2)
                {
                    User? user;
                    try
                    {
                        user = users.Values.SingleOrDefault(u => u.Username == s);
                    }
                    catch
                    {
                        throw new AmbiguousMatchException("Too many users found.");
                    }
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
        }

        throw new EntityNotFoundException("The user was not found.");
    }
}