using NetCord.Rest;

namespace NetCord.Services.ApplicationCommands;

public interface IChoicesProvider<TContext> where TContext : IApplicationCommandContext
{
    public ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(SlashCommandParameter<TContext> parameter);
}
