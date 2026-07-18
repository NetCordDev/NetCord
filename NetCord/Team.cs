using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents a group of developers / Discord users who share access to an application's configuration.
/// </summary>
public class Team(JsonModels.JsonTeam jsonModel, RestClient client) : Entity, IJsonModel<JsonModels.JsonTeam>
{
    JsonModels.JsonTeam IJsonModel<JsonModels.JsonTeam>.JsonModel => jsonModel;

    /// <summary>
    /// The team's unique ID.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The team's icon image hash.
    /// </summary>
    public string? IconHash => jsonModel.IconHash;

    /// <summary>
    /// A list of the team's current members.
    /// </summary>
    public IReadOnlyList<TeamUser> Users { get; } = jsonModel.Users.SelectOrEmpty(m => new TeamUser(m, client)).ToArray();

    /// <summary>
    /// The team's name.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The ID corresponding to the team's owner.
    /// </summary>
    public ulong OwnerId => jsonModel.OwnerId;

    /// <summary>
    /// Gets the <see cref="ImageUrl"/> of the team's icon.
    /// </summary>
    /// <param name="format">The format of the returned <see cref="ImageUrl"/>. Defaults to <see cref="ImageFormat.PNG"/> (or <see cref="ImageFormat.GIF"/> for animated icons).</param>
    /// <returns>An <see cref="ImageUrl"/> pointing to the team's icon. If the user does not have one set, returns <see langword="null"/>.</returns>
    public ImageUrl? GetIconUrl(ImageFormat format) => IconHash is string hash ? ImageUrl.TeamIcon(Id, hash, format) : null;
}
