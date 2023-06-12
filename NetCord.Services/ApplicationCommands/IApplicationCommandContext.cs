namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandContext
{
    public ApplicationCommandInteraction Interaction { get; }
}
