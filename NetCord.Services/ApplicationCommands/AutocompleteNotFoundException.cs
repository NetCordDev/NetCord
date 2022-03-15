namespace NetCord.Services.ApplicationCommands;

public class AutocompleteNotFoundException : Exception
{
    public AutocompleteNotFoundException() : base("Autocomplete not found")
    {
    }
}
