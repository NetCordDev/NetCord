namespace NetCord.Services.ApplicationCommands;

public interface ILocalizationsProvider
{
    public ValueTask<IReadOnlyDictionary<string, string>?> GetLocalizationsAsync(IReadOnlyList<LocalizationPathSegment> path, CancellationToken cancellationToken = default);
}
