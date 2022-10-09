using NetCord.JsonModels;

namespace NetCord.Rest;

public class GatewaySessionStartLimit : IJsonModel<JsonGatewaySessionStartLimit>
{
    JsonGatewaySessionStartLimit IJsonModel<JsonGatewaySessionStartLimit>.JsonModel => _jsonModel;
    private readonly JsonGatewaySessionStartLimit _jsonModel;

    public GatewaySessionStartLimit(JsonGatewaySessionStartLimit jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public int Total => _jsonModel.Total;

    public int Remaining => _jsonModel.Remaining;

    public int ResetAfter => _jsonModel.ResetAfter;

    public int MaxConcurrency => _jsonModel.MaxConcurrency;
}
