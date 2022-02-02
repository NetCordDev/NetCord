namespace NetCord.Services.SlashCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : BaseSlashCommandContext
{
    public abstract Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options);

    public abstract ApplicationCommandOptionType Type { get; }

    public virtual double? GetMaxValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual double? GetMinValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter) => null;
}

internal interface ISlashCommandTypeReader
{
}

public delegate Task<object> SlashCommandTypeReaderDelegate<TContext>(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) where TContext : BaseSlashCommandContext;