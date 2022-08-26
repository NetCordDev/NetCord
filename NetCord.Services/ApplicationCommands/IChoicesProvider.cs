using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IChoicesProvider<TContext> where TContext : IApplicationCommandContext
{
    public IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter);
}
