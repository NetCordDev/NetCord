namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class BooleanTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Boolean;

    public override ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(bool.TryParse(value, out var result) ? SlashCommandTypeReaderResult.Success(result) : SlashCommandTypeReaderResult.ParseFail(parameter.Name));
}
