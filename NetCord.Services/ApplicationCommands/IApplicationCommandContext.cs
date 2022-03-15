namespace NetCord.Services.ApplicationCommands;

public interface IApplicationCommandContext : IContext
{
    public ApplicationCommandInteraction Interaction { get; }
}