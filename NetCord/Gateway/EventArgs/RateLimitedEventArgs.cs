using System.Text.Json;

using NetCord.Gateway.JsonModels.EventArgs;

namespace NetCord.Gateway;

public class RateLimitedEventArgs(JsonRateLimitedEventArgs jsonModel) : IJsonModel<JsonRateLimitedEventArgs>
{
    JsonRateLimitedEventArgs IJsonModel<JsonRateLimitedEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// Gateway opcode of the event that was rate limited.
    /// </summary>
    public GatewayOpcode Opcode => jsonModel.Opcode;

    /// <summary>
    /// The number of seconds to wait before submitting another request.
    /// </summary>
    public double RetryAfter => jsonModel.RetryAfter;

    /// <summary>
    /// Metadata for the event that was rate limited.
    /// </summary>
    public RateLimitMetadata Metadata { get; } = jsonModel.Opcode switch
    {
        GatewayOpcode.RequestGuildUsers => new RequestGuildUsersRateLimitMetadata(jsonModel.Metadata.ToObject(Serialization.Default.JsonRequestGuildUsersRateLimitMetadata)),
        _ => new UnknownRateLimitMetadata(jsonModel.Metadata),
    };
}

public abstract class RateLimitMetadata;

public class RequestGuildUsersRateLimitMetadata(JsonRequestGuildUsersRateLimitMetadata jsonModel) : RateLimitMetadata
{
    /// <summary>
    /// The ID of the guild to get users for.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The nonce to identify the response.
    /// </summary>
    public string? Nonce => jsonModel.Nonce;
}

public interface IUnknownRateLimitMetadata
{
    /// <summary>
    /// The raw data of the rate limit metadata.
    /// </summary>
    public JsonElement Data { get; }
}

internal class UnknownRateLimitMetadata(JsonElement data) : RateLimitMetadata, IUnknownRateLimitMetadata
{
    public JsonElement Data => data;
}
