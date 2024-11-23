using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace NetCord.Rest;

/// <summary>
/// 
/// </summary>
/// <param name="buttons">Buttons of the action row (max 5).</param>
[CollectionBuilder(typeof(ActionRowProperties), nameof(Create))]
public partial class ActionRowProperties(IEnumerable<IButtonProperties> buttons) : ComponentProperties, IEnumerable<IButtonProperties>
{
    public ActionRowProperties() : this([])
    {
    }

    public override ComponentType ComponentType => ComponentType.ActionRow;

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
