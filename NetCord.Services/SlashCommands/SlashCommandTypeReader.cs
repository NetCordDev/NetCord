namespace NetCord.Services.SlashCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : ISlashCommandContext
{
    public abstract Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options);

    public abstract ApplicationCommandOptionType Type { get; }

    public virtual double? GetMaxValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual double? GetMinValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual IEnumerable<ApplicationCommandOptionChoiceProperties>? GetChoices(SlashCommandParameter<TContext> parameter) => null;

    public virtual IAutocompleteProvider? GetAutocompleteProvider(SlashCommandParameter<TContext> parameter) => null;

    public virtual IEnumerable<ChannelType>? GetAllowedChannelTypes(SlashCommandParameter<TContext> parameter) => null;
}

internal interface ISlashCommandTypeReader
{
}