using System.Collections.Immutable;

namespace NetCord.Rest;

public class GuildWidget : Entity
{
    private readonly JsonModels.JsonGuildWidget _jsonModel;

    public override ulong Id => _jsonModel.Id;

    public string Name => _jsonModel.Name;

    public string? InstantInvite => _jsonModel.InstantInvite;

    public IReadOnlyDictionary<ulong, GuildWidgetChannel> Channels { get; }

    public IReadOnlyDictionary<ulong, User> Users { get; }

    public int PresenceCount => _jsonModel.PresenceCount;

    public GuildWidget(JsonModels.JsonGuildWidget jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        Channels = _jsonModel.Channels.ToDictionary(c => c.Id, c => new GuildWidgetChannel(c));
        Users = _jsonModel.Users.ToImmutableDictionary(u => u.Id, u => new User(u, client));
    }
}
