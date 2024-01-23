using NetCord.Rest;

namespace NetCord;

public partial class GuildSticker : Sticker
{
    private readonly RestClient _client;

    public bool? Available => _jsonModel.Available;

    public ulong GuildId => _jsonModel.GuildId;

    public User? Creator { get; }

    public GuildSticker(JsonModels.JsonSticker jsonModel, RestClient client) : base(jsonModel)
    {
        _client = client;

        var creator = jsonModel.Creator;
        if (creator is not null)
            Creator = new(creator, client);
    }
}
