using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class InteractionResolvedData
{
    public IReadOnlyDictionary<ulong, User>? Users { get; }

    public IReadOnlyDictionary<ulong, Role>? Roles { get; }

    public IReadOnlyDictionary<ulong, Channel>? Channels { get; }

    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    public InteractionResolvedData(JsonInteractionResolvedData jsonModel, ulong? guildId, RestClient client)
    {
        var users = jsonModel.Users;
        if (users is not null)
        {
            var guildUsers = jsonModel.GuildUsers;
            if (guildUsers is not null)
            {
                Dictionary<ulong, User> resultUsers = [];
                using (var enumerator = users.GetEnumerator())
                {
                    var max = users.Count - guildUsers.Count;
                    for (var i = 0; i < max; i++)
                    {
                        enumerator.MoveNext();
                        var current = enumerator.Current;
                        resultUsers.Add(current.Key, new(current.Value, client));
                    }

                    var guildIdValue = guildId.GetValueOrDefault();
                    foreach (var guildUser in guildUsers)
                    {
                        enumerator.MoveNext();
                        var current = enumerator.Current;

                        var guildUserModel = guildUser.Value;
                        guildUserModel.User = current.Value;
                        resultUsers.Add(current.Key, new GuildInteractionUser(guildUserModel, guildIdValue, client));
                    }
                }
                Users = resultUsers;
            }
            else
                Users = users.ToDictionary(u => u.Key, u => new User(u.Value, client));
        }

        var roles = jsonModel.Roles;
        if (roles is not null)
            Roles = roles.ToDictionary(r => r.Key, r => new Role(r.Value, guildId.GetValueOrDefault(), client));

        var channels = jsonModel.Channels;
        if (channels is not null)
            Channels = channels.ToDictionary(c => c.Key, c => Channel.CreateFromJson(c.Value, client));

        var attachments = jsonModel.Attachments;
        if (attachments is not null)
            Attachments = attachments.ToDictionary(c => c.Key, c => Attachment.CreateFromJson(c.Value));
    }
}
