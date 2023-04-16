namespace NetCord.Services.ApplicationCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : IApplicationCommandContext
{
    public abstract Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration);

    public abstract ApplicationCommandOptionType Type { get; }

    public virtual double? GetMaxValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual double? GetMinValue(SlashCommandParameter<TContext> parameter) => null;

    public virtual int? GetMaxLength(SlashCommandParameter<TContext> parameter) => null;

    public virtual int? GetMinLength(SlashCommandParameter<TContext> parameter) => null;

    public virtual IChoicesProvider<TContext>? ChoicesProvider => null;

    public virtual Type? AutocompleteProviderType => null;

    public virtual ITranslationsProvider? NameTranslationsProvider => null;

    public virtual ITranslationsProvider? DescriptionTranslationsProvider => null;

    public virtual IEnumerable<ChannelType>? AllowedChannelTypes => null;
}

internal interface ISlashCommandTypeReader
{
}
