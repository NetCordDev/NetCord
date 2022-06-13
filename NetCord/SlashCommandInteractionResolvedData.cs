namespace NetCord;

public class SlashCommandInteractionResolvedData
{
    public IReadOnlyDictionary<Snowflake, User>? Users { get; }

    public IReadOnlyDictionary<Snowflake, GuildRole>? Roles { get; }

    public IReadOnlyDictionary<Snowflake, Channel>? Channels { get; }

    public IReadOnlyDictionary<Snowflake, Attachment>? Attachments { get; }

    public SlashCommandInteractionResolvedData(JsonModels.JsonApplicationCommandResolvedData jsonModel, Snowflake? guildId, RestClient client)
    {
        if (jsonModel.Users != null)
        {
            if (jsonModel.GuildUsers != null)
            {
                var enumerator = jsonModel.Users.GetEnumerator();
                int max = jsonModel.Users.Count - jsonModel.GuildUsers.Count;
                Dictionary<Snowflake, User> users = new();
                for (int i = 0; i < max; i++)
                {
                    enumerator.MoveNext();
                    var current = enumerator.Current;
                    users.Add(current.Key, new(current.Value, client));
                }
                foreach (var guildUser in jsonModel.GuildUsers)
                {
                    enumerator.MoveNext();
                    var current = enumerator.Current;
                    users.Add(current.Key, new GuildInteractionUser(guildUser.Value with { User = current.Value }, guildId.GetValueOrDefault(), client));
                }
                enumerator.Dispose();
                Users = users;
            }
            else
                Users = jsonModel.Users.ToDictionary(u => u.Key, u => new User(u.Value, client));
        }

        if (jsonModel.Roles != null)
            Roles = jsonModel.Roles.ToDictionary(r => r.Key, r => new GuildRole(r.Value, client));
        if (jsonModel.Channels != null)
            Channels = jsonModel.Channels.ToDictionary(c => c.Key, c => Channel.CreateFromJson(c.Value, client));
        if (jsonModel.Attachments != null)
            Attachments = jsonModel.Attachments.ToDictionary(c => c.Key, c => Attachment.CreateFromJson(c.Value));
    }
}