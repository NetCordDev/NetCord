using System.Globalization;

using NetCord.Services.Utils;

namespace NetCord.Services.Interactions.TypeReaders;

public class GuildUserTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        var guild = context.Interaction.Guild;
        if (guild is null)
            goto NotFound;
        var users = guild.Users;
        var span = input.Span;
        var s = span.ToString();

        // by id
        if (Snowflake.TryParse(s, out ulong id))
        {
            if (users.TryGetValue(id, out var user))
                return new(TypeReaderResult.Success(user));
        }
        else
        {
            // by mention
            if (Mention.TryParseUser(span, out id))
            {
                if (users.TryGetValue(id, out var user))
                    return new(TypeReaderResult.Success(user));
                goto NotFound;
            }

            // by name and tag
            if (span.Length is >= 7 and <= 37 && span[^5] == '#')
            {
                var username = span[..^5].ToString();
                if (ushort.TryParse(span[^4..], NumberStyles.None, CultureInfo.InvariantCulture, out var discriminator))
                {
                    var user = users.Values.FirstOrDefault(u => u.Username == username && u.Discriminator == discriminator);
                    if (user is not null)
                        return new(TypeReaderResult.Success(user));
                }
                goto NotFound;
            }
        }

        // by name or nickname
        {
            var len = input.Length;
            if (len <= 32)
            {
                if (users.Values.TryGetSingle(len >= 2 ? u => u.Username == s || u.Nickname == s : u => u.Nickname == s, out var user))
                    return new(TypeReaderResult.Success(user));

                if (user is not null)
                    return new(TypeReaderResult.Fail("Too many users found."));
            }
        }
        NotFound:
        return new(TypeReaderResult.Fail("The user was not found."));
    }
}
