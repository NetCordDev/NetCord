using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an application for a guild integration.
/// </summary>
public class IntegrationApplication(JsonIntegrationApplication jsonModel, RestClient client) : Entity, IJsonModel<JsonIntegrationApplication>
{
    JsonIntegrationApplication IJsonModel<JsonIntegrationApplication>.JsonModel => jsonModel;

    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    public override ulong Id => jsonModel.Id;

    /// <summary>
    /// The name of the application.
    /// </summary>
    public string Name => jsonModel.Name;

    /// <summary>
    /// The icon hash of the application.
    /// </summary>
    public string? IconHash => jsonModel.IconHash;

    /// <summary>
    /// The description of the application.
    /// </summary>
    public string Description => jsonModel.Description;

    /// <summary>
    /// The bot user associated with this application.
    /// </summary>
    public User? Bot { get; } = jsonModel.Bot is { } bot ? new(bot, client) : null;
}
