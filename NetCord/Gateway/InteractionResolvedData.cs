using NetCord.Rest;

namespace NetCord.Gateway;

public class InteractionResolvedData
{
    public IReadOnlyDictionary<ulong, User>? Users { get; }

    public IReadOnlyDictionary<ulong, GuildRole>? Roles { get; }

    public IReadOnlyDictionary<ulong, Channel>? Channels { get; }

    public IReadOnlyDictionary<ulong, Attachment>? Attachments { get; }

    public InteractionResolvedData(JsonModels.JsonApplicationCommandResolvedData jsonModel, ulong? guildId, RestClient client)
    {
        if (jsonModel.Users != null)
        {
            if (jsonModel.GuildUsers != null)
            {
                Dictionary<ulong, User> users = new();
                using (var enumerator = jsonModel.Users.GetEnumerator())
                {
                    var max = jsonModel.Users.Count - jsonModel.GuildUsers.Count;
                    for (var i = 0; i < max; i++)
                    {
                        enumerator.MoveNext();
                        var current = enumerator.Current;
                        users.Add(current.Key, new(current.Value, client));
                    }
                    foreach (var guildUser in jsonModel.GuildUsers)
                    {
                        enumerator.MoveNext();
                        var current = enumerator.Current;
                        guildUser.Value.User = current.Value;
                        users.Add(current.Key, new GuildInteractionUser(guildUser.Value, guildId.GetValueOrDefault(), client));
                    }
                }
                Users = users;
            }
            else
                Users = jsonModel.Users.ToDictionary(u => u.Key, u => new User(u.Value, client));
        }

        if (jsonModel.Roles != null)
            Roles = jsonModel.Roles.ToDictionary(r => r.Key, r => new GuildRole(r.Value, guildId.GetValueOrDefault(), client));
        if (jsonModel.Channels != null)
            Channels = jsonModel.Channels.ToDictionary(c => c.Key, c => Channel.CreateFromJson(c.Value, client));
        if (jsonModel.Attachments != null)
            Attachments = jsonModel.Attachments.ToDictionary(c => c.Key, c => Attachment.CreateFromJson(c.Value));
    }
}
