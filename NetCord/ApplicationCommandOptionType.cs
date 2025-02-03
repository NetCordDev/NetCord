namespace NetCord;

/// <summary>
/// Represents a command parameter's type.
/// </summary>
public enum ApplicationCommandOptionType
{
    /// <summary>
    /// A sub-command.
    /// </summary>
    /// <remarks>
    /// If present, the root <see cref="ApplicationCommandInteraction"/> can no longer be invoked.
    /// </remarks>
    SubCommand = 1,

    /// <summary>
    /// A group of sub-commands.
    /// </summary>
    /// <remarks>
    /// If present, the root <see cref="ApplicationCommandInteraction"/> can no longer be invoked.
    /// </remarks>
    SubCommandGroup = 2,

    /// <summary>
    /// A <see cref="string"/> value.
    /// </summary>
    String = 3,

    /// <summary>
    /// An integral number in the range <c>-2^53</c> to <c>2^53</c>.
    /// </summary>
    Integer = 4,

    /// <summary>
    /// A <see langword="true"/> or <see langword="false"/> value.
    /// </summary>
    Boolean = 5,

    /// <summary>
    /// Any user.
    /// </summary>
    User = 6,

    /// <summary>
    /// Any channel or category in the current guild.
    /// </summary>
    Channel = 7,

    /// <summary>
    /// Any role in the current guild.
    /// </summary>
    Role = 8,

    /// <summary>
    /// Any role in the current guild, or any user.
    /// </summary>
    Mentionable = 9,

    /// <summary>
    /// A floating point number in the range <c>-2^53</c> to <c>2^53</c>.
    /// </summary>
    Double = 10,

    /// <summary>
    /// A <see cref="NetCord.Attachment"/> object.
    /// </summary>
    Attachment = 11,
}
