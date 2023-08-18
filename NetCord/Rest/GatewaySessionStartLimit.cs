namespace NetCord.Rest;

public class GatewaySessionStartLimit : IJsonModel<JsonModels.JsonGatewaySessionStartLimit>
{
    JsonModels.JsonGatewaySessionStartLimit IJsonModel<JsonModels.JsonGatewaySessionStartLimit>.JsonModel => _jsonModel;
    private readonly JsonModels.JsonGatewaySessionStartLimit _jsonModel;

    public GatewaySessionStartLimit(JsonModels.JsonGatewaySessionStartLimit jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public int Total => _jsonModel.Total;

    public int Remaining => _jsonModel.Remaining;

    public TimeSpan ResetAfter => new(_jsonModel.ResetAfter * TimeSpan.TicksPerMillisecond);

    public int MaxConcurrency => _jsonModel.MaxConcurrency;
}
