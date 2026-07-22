using NetCord.JsonModels;

namespace NetCord;

/// <summary>
/// Represents the current user within an already joined thread.
/// </summary>
public class ThreadCurrentUser(JsonThreadCurrentUser jsonModel) : IJsonModel<JsonThreadCurrentUser>
{
    JsonThreadCurrentUser IJsonModel<JsonThreadCurrentUser>.JsonModel => jsonModel;

    /// <inheritdoc cref="ThreadUser.JoinTimestamp"/>
    public DateTimeOffset JoinTimestamp => jsonModel.JoinTimestamp;

    /// <inheritdoc cref="ThreadUser.Flags"/>
    public ThreadUserFlags Flags => jsonModel.Flags;
}
