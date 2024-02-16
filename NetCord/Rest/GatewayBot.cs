namespace NetCord.Rest;

public class GatewayBot(JsonModels.JsonGatewayBot jsonModel) : IJsonModel<JsonModels.JsonGatewayBot>
{
    JsonModels.JsonGatewayBot IJsonModel<JsonModels.JsonGatewayBot>.JsonModel => jsonModel;

    public string Url => jsonModel.Url;

    public int ShardCount => jsonModel.ShardCount;

    public GatewaySessionStartLimit SessionStartLimit { get; } = new(jsonModel.SessionStartLimit);
}
