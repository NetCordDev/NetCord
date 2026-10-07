namespace NetCord.Rest;

public class GatewayBot(JsonModels.JsonGatewayBot jsonModel)
{
    public string Url { get; } = jsonModel.Url;

    public int ShardCount { get; } = jsonModel.ShardCount;

    public GatewaySessionStartLimit SessionStartLimit { get; } = new(jsonModel.SessionStartLimit);
}
