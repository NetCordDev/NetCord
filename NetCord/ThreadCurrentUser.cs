using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// A minimal <see cref="ThreadUser"/>, sent for threads within the <see cref="Gateway.GatewayClient.GuildCreate"/> event.
/// </summary>
public class ThreadCurrentUser(JsonThreadCurrentUser jsonModel) : IJsonModel<JsonThreadCurrentUser>
{
    JsonThreadCurrentUser IJsonModel<JsonThreadCurrentUser>.JsonModel => jsonModel;

    /// <inheritdoc cref="ThreadUser.JoinTimestamp"/>
    public DateTimeOffset JoinTimestamp => jsonModel.JoinTimestamp;

    /// <inheritdoc cref="ThreadUser.Flags"/>
    public int Flags => jsonModel.Flags;
}
