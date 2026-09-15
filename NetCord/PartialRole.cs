using System.ComponentModel;

using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a partial role.
/// </summary>
/// <remarks>
/// Useful for <see cref="Rest.RestInvite"/>.
/// </remarks>
public partial class PartialRole(JsonPartialRole jsonModel, ulong guildId, RestClient client) : ClientEntity(client), IJsonModel<JsonPartialRole>
{
    JsonPartialRole IJsonModel<JsonPartialRole>.JsonModel => jsonModel;

    /// <summary>
    /// The role's ID.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the role.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The role's colors.
    /// </summary>
    public RoleColors Colors => new(jsonMode.Colors);

    /// <summary>
    /// The raw position of this role.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Position"/> to get a properly comparable and sortable position value.
    /// </remarks>
    public int RawPosition => jsonModel.Position;

    /// <summary>
    /// The position of this role for sorting and comparing.
    /// </summary>
    public RolePosition Position => new(RawPosition, Id);

    /// <summary>
    /// The role's icon hash.
    /// </summary>
    public string? IconHash => jsonModel.IconHash;

    /// <summary>
    /// The role's Unicode emoji.
    /// </summary>
    public string? UnicodeEmoji => jsonModel.UnicodeEmoji;

    /// <summary>
    /// The ID of the guild this role belongs to.
    /// </summary>
    public ulong GuildId => guildId;
    
    public override string ToString() => $"<@&{Id}>";
}
