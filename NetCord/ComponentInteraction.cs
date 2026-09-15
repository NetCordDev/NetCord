using NetCord.Gateway;
using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Acts as a base class for component interactions, such as button clicks and select menu selections.
/// </summary>
public abstract class ComponentInteraction(JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) : Interaction(jsonModel, guild, sendResponseAsync, client)
{
    /// <summary>
    /// Holds the containing component interaction's data.
    /// </summary>
    public abstract override ComponentInteractionData Data { get; }
}

/// <summary>
/// Contains data for an invoked component interaction.
/// </summary>
public class ComponentInteractionData(JsonInteractionData jsonModel) : InteractionData(jsonModel)
{
    /// <summary>
    /// The developer-defined identifier for the component.
    /// </summary>
    public string CustomId => jsonModel.CustomId!;
}
