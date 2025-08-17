using System.Text.Json;

using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class RateLimitedEventArgs(JsonRateLimitedEventArgs jsonModel) : IJsonModel<JsonRateLimitedEventArgs>
{
    JsonRateLimitedEventArgs IJsonModel<JsonRateLimitedEventArgs>.JsonModel => jsonModel;

    public GatewayOpcode Opcode => jsonModel.Opcode;

    public double RetryAfter => jsonModel.RetryAfter;

    public RateLimitMetadata Metadata { get; } = jsonModel.Opcode switch
    {
        GatewayOpcode.RequestGuildUsers => new RequestGuildUsersRateLimitMetadata(jsonModel.Metadata.ToObject(Serialization.Default.JsonRequestGuildUsersRateLimitMetadata)),
        _ => new UnknownRateLimitMetadata(jsonModel.Metadata),
    };
}

public abstract class RateLimitMetadata;

public class RequestGuildUsersRateLimitMetadata(JsonRequestGuildUsersRateLimitMetadata jsonModel) : RateLimitMetadata
{
    public ulong GuildId => jsonModel.GuildId;

    public string? Nonce => jsonModel.Nonce;
}

public interface IUnknownRateLimitMetadata
{
    public JsonElement Data { get; }
}

internal class UnknownRateLimitMetadata(JsonElement data) : RateLimitMetadata, IUnknownRateLimitMetadata
{
    public JsonElement Data => data;
}
