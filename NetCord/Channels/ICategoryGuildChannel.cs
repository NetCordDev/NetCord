namespace NetCord;

/// <summary>
/// Represents an organizational category that contains up to 50 channels.
/// </summary>
public interface ICategoryGuildChannel :
    IGuildChannel, IPermissionOverwriteChannel, INamedChannel, IPositionedGuildChannel
{
}
