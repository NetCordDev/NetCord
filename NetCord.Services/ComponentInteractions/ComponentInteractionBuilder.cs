namespace NetCord.Services.ComponentInteractions;

/// <inheritdoc cref="ComponentInteractionAttribute" />
/// <param name="customId" />
/// <param name="handler"><inheritdoc cref="Handler" path="/summary" /></param>
[GenerateMethodsForProperties]
public partial class ComponentInteractionBuilder(string customId, Delegate handler)
{
    /// <inheritdoc cref="ComponentInteractionAttribute.CustomId" />
    public string CustomId => customId;

    /// <summary>
    /// Handler that represents the body of the component interaction.
    /// </summary>
    public Delegate Handler => handler;
}
