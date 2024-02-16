namespace NetCord.Rest;

public class GatewaySessionStartLimit(JsonModels.JsonGatewaySessionStartLimit jsonModel) : IJsonModel<JsonModels.JsonGatewaySessionStartLimit>
{
    JsonModels.JsonGatewaySessionStartLimit IJsonModel<JsonModels.JsonGatewaySessionStartLimit>.JsonModel => jsonModel;

    public int Total => jsonModel.Total;

    public int Remaining => jsonModel.Remaining;

    public TimeSpan ResetAfter => new(jsonModel.ResetAfter * TimeSpan.TicksPerMillisecond);

    public int MaxConcurrency => jsonModel.MaxConcurrency;
}
