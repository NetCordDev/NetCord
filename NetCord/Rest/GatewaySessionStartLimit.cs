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

    public int ResetAfter => _jsonModel.ResetAfter;

    public int MaxConcurrency => _jsonModel.MaxConcurrency;
}
