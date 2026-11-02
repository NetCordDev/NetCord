using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildIncidentsData(JsonGuildIncidentsData jsonModel)
{
    public DateTimeOffset? InvitesDisabledUntil { get; } = jsonModel.InvitesDisabledUntil;

    public DateTimeOffset? DmsDisabledUntil { get; } = jsonModel.DmsDisabledUntil;

    public DateTimeOffset? DmSpamDetectedAt { get; } = jsonModel.DmSpamDetectedAt;

    public DateTimeOffset? RaidDetectedAt { get; } = jsonModel.RaidDetectedAt;
}

