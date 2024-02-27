namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandContext : IInteractionContext
{
    public new ApplicationCommandInteraction Interaction { get; }

    Interaction IInteractionContext.Interaction => Interaction;
}
