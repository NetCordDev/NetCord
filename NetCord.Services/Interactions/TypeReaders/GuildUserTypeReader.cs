using System.Reflection;

namespace NetCord.Services.Interactions.TypeReaders;

public class GuildUserTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(string input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceOptions<TContext> options)
    {
        var guild = context.Interaction.Guild;
        if (guild == null)
            goto exception;
        IReadOnlyDictionary<DiscordId, GuildUser> users = guild.Users;
        // by id
        if (DiscordId.TryCreate(input, out DiscordId id))
        {
            if (users.TryGetValue(id, out var user))
                return Task.FromResult((object?)user);
        }
        else
        {
            var span = input.AsSpan();
            // by mention
            if (MentionUtils.TryParseUser(span, out id))
            {
                if (users.TryGetValue(id, out var user))
                    return Task.FromResult((object?)user);
                goto exception;
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], out var discriminator))
                {
                    var user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user != null)
                        return Task.FromResult((object?)user);
                }
                goto exception;
            }
        }

        // by name or nickname
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
                    return Task.FromResult((object?)user);
            }
        }
        exception:
        throw new EntityNotFoundException("The user was not found");
    }
}