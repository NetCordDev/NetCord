using System.Globalization;

namespace NetCord.Services.ApplicationCommands;

public interface ITranslateProvider
{
    public IReadOnlyDictionary<CultureInfo, string>? Translations { get; }
}