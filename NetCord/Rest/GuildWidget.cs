using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildWidget(JsonGuildWidget jsonModel, RestClient client) : Entity(jsonModel)
{
    public string Name { get; } = jsonModel.Name;

    public string? InstantInvite { get; } = jsonModel.InstantInvite;

    public IReadOnlyDictionary<ulong, GuildWidgetChannel> Channels { get; } = jsonModel.Channels.ToDictionary(c => c.Id, c => new GuildWidgetChannel(c));

    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionary(u => u.Id, u => new User(u, client));

    public int PresenceCount { get; } = jsonModel.PresenceCount;
}
