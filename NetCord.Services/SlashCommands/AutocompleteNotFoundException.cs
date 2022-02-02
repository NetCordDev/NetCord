namespace NetCord.Services.SlashCommands;

public class AutocompleteNotFoundException : Exception
{
    public AutocompleteNotFoundException() : base("Autocomplete not found")
    {
    }
}
