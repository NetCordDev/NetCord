using NetCord.Rest;

namespace NetCord;

public class PingInteraction : ClientEntity, IInteraction
{
    JsonModels.JsonInteraction IJsonModel<JsonModels.JsonInteraction>.JsonModel => _jsonModel;
    private protected readonly JsonModels.JsonInteraction _jsonModel;

    public PingInteraction(JsonModels.JsonInteraction jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        var guildId = jsonModel.GuildId;
        if (guildId.HasValue)
            User = new GuildInteractionUser(jsonModel.GuildUser!, guildId.GetValueOrDefault(), client);
        else
            User = new(jsonModel.User!, client);
    }

    public override ulong Id => _jsonModel.Id;

    public ulong ApplicationId => _jsonModel.ApplicationId;

    public User User { get; }

    public string Token => _jsonModel.Token;
}
