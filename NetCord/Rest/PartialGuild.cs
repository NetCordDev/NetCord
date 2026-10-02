using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public partial class PartialGuild(JsonPartialGuild jsonModel, RestClient client) : ClientEntity(client)
{
    /// <summary>
    /// The guild's ID.
    /// </summary>
    public override ulong Id { get; } = jsonModel.Id;

    /// <summary>
    /// The name of the <see cref="RestGuild"/>. Must be between 2 and 100 characters. Leading and trailing whitespace are trimmed.
    /// </summary>
    public string Name { get; } = jsonModel.Name;

    /// <summary>
    /// Whether the guild has an icon set.
    /// </summary>
    public bool HasIcon => IconHash is not null;

    /// <summary>
    /// The guild's icon hash.
    /// </summary>
    public string? IconHash { get; } = jsonModel.IconHash;

    /// <summary>
    /// Whether the guild has a set splash.
    /// </summary>
    public bool HasSplash => SplashHash is not null;

    /// <summary>
    /// The guild's splash hash.
    /// </summary>
    public string? SplashHash { get; } = jsonModel.SplashHash;

    /// <summary>
    /// Whether the guild has a set banner.
    /// </summary>
    public bool HasBanner => BannerHash is not null;

    /// <summary>
    /// The guild's banner hash.
    /// </summary>
    public string? BannerHash { get; } = jsonModel.BannerHash;

    /// <summary>
    /// The guild's description, shown in the 'Discovery' tab.
    /// </summary>
    public string? Description { get; } = jsonModel.Description;

    /// <summary>
    /// A list of guild feature strings, representing what features are currently enabled.
    /// </summary>
    public IReadOnlyList<string>? Features { get; } = jsonModel.Features;

    /// <summary>
    /// The <see cref="NetCord.VerificationLevel"/> required for the guild.
    /// </summary>
    public VerificationLevel? VerificationLevel { get; } = jsonModel.VerificationLevel;

    /// <summary>
    /// The guild's vanity invite URL code.
    /// </summary>
    public string? VanityUrlCode { get; } = jsonModel.VanityUrlCode;

    /// <summary>
    /// The welcome screen shown to new members, returned in an invite's <see cref="RestGuild"/> object.
    /// </summary>
    public GuildWelcomeScreen? WelcomeScreen { get; } = jsonModel.WelcomeScreen is { } welcomeScreen ? new(welcomeScreen) : null;
}

