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
