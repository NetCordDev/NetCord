using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

#pragma warning disable IDE0028 // Simplify collection initialization
#pragma warning disable IDE0306 // Simplify collection initialization

/// <summary>
/// 
/// </summary>
/// <param name="buttons">Buttons of the action row (max 5).</param>
[CollectionBuilder(typeof(ActionRowProperties), nameof(Create))]
public partial class ActionRowProperties(IEnumerable<IButtonProperties> buttons) : IComponentProperties, IEnumerable<IButtonProperties>
{
    public ActionRowProperties() : this([])
    {
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("id")]
    public int? Id { get; set; }

    [JsonPropertyName("type")]
    public ComponentType ComponentType => ComponentType.ActionRow;

    /// <summary>
    /// Buttons of the action row (max 5).
    /// </summary>
    [JsonPropertyName("components")]
    public IEnumerable<IButtonProperties> Buttons { get; set; } = buttons;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Add(IButtonProperties button) => AddButtons(button);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ActionRowProperties Create(ReadOnlySpan<IButtonProperties> buttons) => new(buttons.ToArray());

    IEnumerator<IButtonProperties> IEnumerable<IButtonProperties>.GetEnumerator() => Buttons.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Buttons).GetEnumerator();
}
