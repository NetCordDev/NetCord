using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public partial class ApplicationEmoji(JsonEmoji jsonModel, ulong applicationId, RestClient client) : CustomEmoji(jsonModel, client)
{
    public ulong ApplicationId { get; } = applicationId;
}
