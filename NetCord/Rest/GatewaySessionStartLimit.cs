using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GatewaySessionStartLimit(JsonGatewaySessionStartLimit jsonModel)
{
    public int Total { get; } = jsonModel.Total;

    public int Remaining { get; } = jsonModel.Remaining;

    public TimeSpan ResetAfter { get; } = TimeSpan.FromMilliseconds(jsonModel.ResetAfter);

    public int MaxConcurrency { get; } = jsonModel.MaxConcurrency;
}
