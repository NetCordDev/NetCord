namespace NetCord.Services.ApplicationCommands;

public interface ITranslationsProvider
{
    public IReadOnlyDictionary<string, string>? Translations { get; }
}
