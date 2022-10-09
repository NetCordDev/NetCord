using NetCord.JsonModels;

namespace NetCord.Rest;

public class GatewayBot : IJsonModel<JsonGatewayBot>
{
    JsonGatewayBot IJsonModel<JsonGatewayBot>.JsonModel => _jsonModel;
    private readonly JsonGatewayBot _jsonModel;

    public GatewayBot(JsonGatewayBot jsonModel)
    {
        _jsonModel = jsonModel;
        SessionStartLimit = new(jsonModel.SessionStartLimit);
    }

    public string Url => _jsonModel.Url;

    public int Shards => _jsonModel.Shards;

    public GatewaySessionStartLimit SessionStartLimit { get; }
}
