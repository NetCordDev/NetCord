namespace NetCord;

/// <summary>
/// Represents a mentioned channel (such as <c>&lt;#1060153373401288816&gt;</c>).
/// </summary>
public class GuildChannelMention(JsonModels.JsonGuildChannelMention jsonModel) : Entity
{
    /// <summary>
    /// The guild channel's ID.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The containing guild's ID.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The guild channel's type.
    /// </summary>
    public ChannelType Type => jsonModel.Type;

    /// <summary>
    /// The guild channel's name.
    /// </summary>
    public string Name => jsonModel.Name;
}
