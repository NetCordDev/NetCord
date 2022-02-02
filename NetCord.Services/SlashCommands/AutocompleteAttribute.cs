namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class AutocompleteAttribute : Attribute
{
    public Type AutocompleteProviderType { get; }

    public AutocompleteAttribute(Type autocompleteProviderType)
    {
        if (!autocompleteProviderType.IsAssignableTo(typeof(IAutocompleteProvider)))
            throw new ArgumentException($"Parameter must inherit from {nameof(IAutocompleteProvider)}", nameof(autocompleteProviderType));

        AutocompleteProviderType = autocompleteProviderType;
    }
}