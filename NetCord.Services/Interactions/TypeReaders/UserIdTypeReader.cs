using System.Reflection;

namespace NetCord.Services.Interactions.TypeReaders;

public class UserIdTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options)
    {
        var guild = context.Interaction.Guild;
        if (guild != null)
        {
            IReadOnlyDictionary<Snowflake, GuildUser> users = guild.Users;
            // by id
            if (Snowflake.TryCreate(input, out Snowflake id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }

            var span = input.AsSpan();

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
                        user = users.Values.SingleOrDefault(len >= 2 ? u => u.Username == input || u.Nickname == input : u => u.Nickname == input);
                    }
                    catch
                    {
                        throw new AmbiguousMatchException("Too many users found");
                    }
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
        }
        else if (context.Interaction.Channel is DMChannel dm)
        {
            IReadOnlyDictionary<Snowflake, User> users = dm.Users;
            // by id
            if (Snowflake.TryCreate(input, out Snowflake id))
            {
                users.TryGetValue(id, out var user);
                return Task.FromResult((object?)new UserId(id, user));
            }

            var span = input.AsSpan();

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
                        user = users.Values.SingleOrDefault(u => u.Username == input);
                    }
                    catch
                    {
                        throw new AmbiguousMatchException("Too many users found");
                    }
                    if (user != null)
                        return Task.FromResult((object?)new UserId(user.Id, user));
                }
            }
        }

        throw new EntityNotFoundException("The user was not found");
    }
}