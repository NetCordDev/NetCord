using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Acts as a base class for application commands, such as slash commands and message commands.
/// </summary>
public abstract class ApplicationCommandInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client)
    : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    /// <summary>
    /// Holds the containing application command's data.
    /// </summary>
    public abstract override ApplicationCommandInteractionData Data { get; }
}

/// <summary>
/// Contains data for an invoked <see cref="ApplicationCommand"/>.
/// </summary>
public class ApplicationCommandInteractionData(JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s ID.
    /// </summary>
    public ulong Id => jsonModel.Id.GetValueOrDefault();

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s name.
    /// </summary>
    public string Name => jsonModel.Name!;

    /// <summary>
    /// The invoked <see cref="ApplicationCommand"/>'s type.
    /// </summary>
    public ApplicationCommandType Type => jsonModel.Type.GetValueOrDefault();

    /// <summary>
    /// The ID of the guild the <see cref="ApplicationCommand"/> is registered to.
    /// </summary>
    public ulong? GuildId => jsonModel.GuildId;
}
