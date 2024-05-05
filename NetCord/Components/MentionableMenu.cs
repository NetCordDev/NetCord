using System.Collections;

using NetCord.JsonModels;

namespace NetCord;

public class MentionableMenu : EntityMenu
{
    public MentionableMenu(JsonComponent jsonModel) : base(jsonModel, GetDefaultValues(jsonModel, out var defaultValues))
    {
        DefaultValues = defaultValues;
    }

    private static DefaultValuesWrapper GetDefaultValues(JsonComponent jsonModel, out MentionableMenuDefaultValue[] defaultValues)
        => new(defaultValues = jsonModel.DefaultValues.SelectOrEmpty(d => new MentionableMenuDefaultValue(d)).ToArray());

    public new IReadOnlyList<MentionableMenuDefaultValue> DefaultValues { get; }

    private class DefaultValuesWrapper(MentionableMenuDefaultValue[] defaultValues) : IReadOnlyList<ulong>
    {
        public ulong this[int index] => defaultValues[index].Id;

        public int Count => defaultValues.Length;

        public IEnumerator<ulong> GetEnumerator()
        {
            int length = defaultValues.Length;
            for (int i = 0; i < length; i++)
                yield return defaultValues[i].Id;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
