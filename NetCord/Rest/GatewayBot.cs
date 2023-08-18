namespace NetCord.Rest;

public class GatewayBot : IJsonModel<JsonModels.JsonGatewayBot>
{
    JsonModels.JsonGatewayBot IJsonModel<JsonModels.JsonGatewayBot>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGatewayBot _jsonModel;

    public GatewayBot(JsonModels.JsonGatewayBot jsonModel)
    {
        _jsonModel = jsonModel;
        SessionStartLimit = new(jsonModel.SessionStartLimit);
    }

    public string Url => _jsonModel.Url;

    public int ShardCount => _jsonModel.ShardCount;

    public GatewaySessionStartLimit SessionStartLimit { get; }
}
