using System.Globalization;

namespace NetCord.Services.ApplicationCommands;

public interface ITranslationsProvider
{
    public IReadOnlyDictionary<CultureInfo, string>? Translations { get; }
}