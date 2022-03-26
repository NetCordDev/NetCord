namespace NetCord.Services.ApplicationCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : IApplicationCommandContext
{
    public abstract Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceOptions<TContext> options);

    public abstract ApplicationCommandOptionType Type { get; }

    public virtual double? GetMaxValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual double? GetMinValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual IChoicesProvider<TContext>? ChoicesProvider => null;

    public virtual IAutocompleteProvider? AutocompleteProvider => null;

    public virtual ITranslateProvider? NameTranslateProvider => null;

    public virtual ITranslateProvider? DescriptionTranslateProvider => null;

    public virtual IEnumerable<ChannelType>? AllowedChannelTypes => null;
}

internal interface ISlashCommandTypeReader
{
}