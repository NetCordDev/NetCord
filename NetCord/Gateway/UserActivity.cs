using NetCord.Gateway.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

public class UserActivity(JsonUserActivity jsonModel, ulong guildId, RestClient client)
{
    public string Name { get; } = jsonModel.Name;

    public UserActivityType Type { get; } = jsonModel.Type;

    public string? Url { get; } = jsonModel.Url;

    public DateTimeOffset CreatedAt { get; } = jsonModel.CreatedAt;

    public UserActivityTimestamps? Timestamps { get; } = jsonModel.Timestamps is { } timestamps ? new(timestamps) : null;

    public ulong? ApplicationId { get; } = jsonModel.ApplicationId;

    public UserActivityStatusDisplayType? StatusDisplayType { get; } = jsonModel.StatusDisplayType;

    public string? Details { get; } = jsonModel.Details;

    public string? DetailsUrl { get; } = jsonModel.DetailsUrl;

    public string? State { get; } = jsonModel.State;

    public string? StateUrl { get; } = jsonModel.StateUrl;

    public Emoji? Emoji { get; } = jsonModel.Emoji is { } emoji ? Emoji.Create(emoji, guildId, client) : null;

    public UserActivityParty? Party { get; } = jsonModel.Party is { } party ? new(party) : null;

    public UserActivityAssets? Assets { get; } = jsonModel.Assets is { } assets ? new(assets) : null;

    public UserActivitySecrets? Secrets { get; } = jsonModel.Secrets is { } secrets ? new(secrets) : null;

    public bool? Instance { get; } = jsonModel.Instance;

    public UserActivityFlags? Flags { get; } = jsonModel.Flags;

    public IReadOnlyList<string>? Buttons { get; } = jsonModel.Buttons;

    public ulong GuildId { get; } = guildId;
}

public enum UserActivityType
{
    Playing = 0,
    Streaming = 1,
    Listening = 2,
    Watching = 3,
    Custom = 4,
    Competing = 5,
}

public class UserActivityTimestamps(JsonUserActivityTimestamps jsonModel)
{
    public DateTimeOffset? StartTime { get; } = jsonModel.Start;

    public DateTimeOffset? EndTime { get; } = jsonModel.End;
}

public class UserActivityParty(JsonUserActivityParty jsonModel)
{
    public string? Id { get; } = jsonModel.Id;

    public PartySize? Size { get; } = jsonModel.Size is { } size ? new(size) : null;
}

public class UserActivityAssets(JsonUserActivityAssets jsonModel)
{
    public string? LargeImageId { get; } = jsonModel.LargeImageId;

    public string? LargeText { get; } = jsonModel.LargeText;

    public string? SmallImageId { get; } = jsonModel.SmallImageId;

    public string? SmallText { get; } = jsonModel.SmallText;
}

public class UserActivitySecrets(JsonUserActivitySecrets jsonModel)
{
    public string? Join { get; } = jsonModel.Join;

    public string? Spectate { get; } = jsonModel.Spectate;

    public string? Match { get; } = jsonModel.Match;
}

/// <summary>
/// Flags about an activity's state.
/// </summary>
[Flags]
public enum UserActivityFlags
{
    Instance = 1 << 0,

    Join = 1 << 1,

    Spectate = 1 << 2,

    JoinRequest = 1 << 3,

    Sync = 1 << 4,

    Play = 1 << 5,

    PartyPrivacyFriends = 1 << 6,

    PartyPrivacyVoiceChannel = 1 << 7,

    Embedded = 1 << 8,
}

public enum UserActivityStatusDisplayType
{
    Name = 0,
    State = 1,
    Details = 2,
}
