using NetCord.Gateway;
using NetCord.Rest;

namespace NetCord;

/// <summary>
/// Represents an interaction triggered by selecting values from a string select menu component.
/// </summary>
public class StringMenuInteraction(JsonModels.JsonInteraction jsonModel, Guild? guild, InteractionResponseDelegate sendResponseAsync, RestClient client) 
    : MessageComponentInteraction(jsonModel, guild, sendResponseAsync, client)
{
    /// <inheritdoc />
    public override StringMenuInteractionData Data { get; } = new(jsonModel.Data!);
}

/// <summary>
/// Represents the interaction data associated with a string select menu component interaction.
/// </summary>
public class StringMenuInteractionData(JsonModels.JsonInteractionData jsonModel) : MessageComponentInteractionData(jsonModel)
{
    /// <summary>
    /// A list of string values selected by the invoking user.
    /// </summary>
    public IReadOnlyList<string> SelectedValues => jsonModel.SelectedValues!;
}
