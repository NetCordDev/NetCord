namespace NetCord.Services.ComponentInteractions;

[GenerateMethodsForProperties]
public class ComponentInteractionBuilder(string customId, Delegate handler)
{
    public string CustomId => customId;

    public Delegate Handler => handler;
}
