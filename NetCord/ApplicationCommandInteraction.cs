using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Acts as a base class for application commands, such as slash commands and message commands.
/// </summary>
public abstract class ApplicationCommandInteraction : Interaction
{
    private protected ApplicationCommandInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : base(jsonModel, guild, sendResponseAsync, client)
    {
    }

    /// <summary>
    /// Holds the containing application command's data.
    /// </summary>
    public abstract override ApplicationCommandInteractionData Data { get; }
}

/// <summary>
/// Contains data for an invoked <see cref="ApplicationCommand"/>.
/// </summary>
public class ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s ID.
    /// </summary>
    public ulong Id => _jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s name.
    /// </summary>
    public string Name => _jsonModel.Name!;

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s type.
    /// </summary>
    public ApplicationCommandType Type => _jsonModel.Type.GetValueOrDefault();

    /// <summary>
    /// The ID of the guild the <see cref="ApplicationCommand"/> is registered to.
    /// </summary>
    public ulong? GuildId => _jsonModel.GuildId;
}
