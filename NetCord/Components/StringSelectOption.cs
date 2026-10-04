using NetCord.JsonModels;

namespace NetCord;

public class StringSelectOption(JsonStringSelectOption jsonModel)
{
    public string Label { get; } = jsonModel.Label;

    public string Value { get; } = jsonModel.Value;

    public string? Description { get; } = jsonModel.Description;

    public EmojiReference? Emoji { get; } = jsonModel.Emoji is { } emoji ? new(emoji) : null;

    public bool Default { get; } = jsonModel.Default.GetValueOrDefault();
}
