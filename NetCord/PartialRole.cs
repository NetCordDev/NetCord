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
public class PartialRole : ClientEntity, IJsonModel<JsonPartialRole>
{
    JsonPartialRole IJsonModel<JsonPartialRole>.JsonModel => _jsonModel;
    private readonly JsonPartialRole _jsonModel;

    /// <summary>
    /// The <see cref="PartialRole"/>'s ID.
    /// </summary>
    public override ulong Id => _jsonModel.Id;

    /// <summary>
    /// The name of the <see cref="PartialRole"/>.
    /// </summary>
    public string Name => _jsonModel.Name;

    /// <summary>
    /// The color of the <see cref="PartialRole"/>.
    /// </summary>
    /// <remarks>
    /// This will still be returned by the API, but using <see cref="Colors"/> is recommended when doing requests.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public Color Color => _jsonModel.Color;

    /// <summary>
    /// The <see cref="PartialRole"/>'s colors.
    /// </summary>
    public RoleColors Colors { get; }

    /// <summary>
    /// The raw position of this <see cref="PartialRole"/>.
    /// </summary>
    /// <remarks>
    /// Use <see cref="Position"/> to get a properly comparable and sortable position value.
    /// </remarks>
    public int RawPosition => _jsonModel.Position;

    /// <summary>
    /// The position of this <see cref="PartialRole"/> for sorting and comparing.
    /// </summary>
    public RolePosition Position => new(RawPosition, Id);

    /// <summary>
    /// The <see cref="PartialRole"/>'s icon hash.
    /// </summary>
    public string? IconHash => _jsonModel.IconHash;

    /// <summary>
    /// The <see cref="PartialRole"/>'s Unicode emoji.
    /// </summary>
    public string? UnicodeEmoji => _jsonModel.UnicodeEmoji;
    public override string ToString() => $"<@&{Id}>";

    public PartialRole(JsonRole jsonModel, RestClient client) : base(client)
    {
        _jsonModel = jsonModel;

        Colors = new(jsonModel.Colors);
    }
}
