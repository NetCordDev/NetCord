namespace NetCord;

/// <summary>
/// Represents a permission override for a command in a guild.
/// </summary>
public class ApplicationCommandPermission(JsonModels.JsonApplicationCommandGuildPermission jsonModel) : Entity, IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>
{
    JsonModels.JsonApplicationCommandGuildPermission IJsonModel<JsonModels.JsonApplicationCommandGuildPermission>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the override's relevant role, user, or channel, depending on the override's <see cref="Type"/>. May also be:
    /// <list type="bullet">
    ///     <item>
    ///         <term>
    ///         <see langword="@everyone"/>
    ///         </term>
    ///         <description>
    ///         Where it is equal to the ID of the override's relevant guild.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>
    ///         'All Channels'
    ///         </term>
    ///         <description>
    ///         Where it is equal to the ID of the override's relevant guild - 1.
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// Indicates the scope of the permission override.
    /// </summary>
    public ApplicationCommandGuildPermissionType Type => jsonModel.Type;

    /// <summary>
    /// Indicates whether the override is intended to enable or disable a command.
    /// </summary>
    public bool Permission => jsonModel.Permission;
}
