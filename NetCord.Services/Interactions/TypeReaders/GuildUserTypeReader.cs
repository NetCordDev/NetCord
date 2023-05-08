using System.Globalization;
using System.Reflection;

namespace NetCord.Services.Interactions.TypeReaders;

public class GuildUserTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : InteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Interaction.Guild;
        if (guild == null)
            goto exception;
        IReadOnlyDictionary<ulong, GuildUser> users = guild.Users;
        var span = input.Span;
        var s = span.ToString();

        // by id
        if (ulong.TryParse(s, NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
        {
            if (users.TryGetValue(id, out var user))
                return Task.FromResult<object?>(user);
        }
        else
        {
            // by mention
            if (MentionUtils.TryParseUser(span, out id))
            {
                if (users.TryGetValue(id, out var user))
                    return Task.FromResult<object?>(user);
                goto exception;
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                {
                    var user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user != null)
                        return Task.FromResult<object?>(user);
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
                    user = users.Values.SingleOrDefault(len >= 2 ? u => u.Username == s || u.Nickname == s : u => u.Nickname == s);
                }
                catch
                {
                    throw new AmbiguousMatchException("Too many users found.");
                }
                if (user != null)
                    return Task.FromResult<object?>(user);
            }
        }
        exception:
        throw new EntityNotFoundException("The user was not found.");
    }
}
