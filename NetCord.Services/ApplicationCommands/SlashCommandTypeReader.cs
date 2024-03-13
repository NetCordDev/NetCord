using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : IApplicationCommandContext
{
    public abstract ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);

    public abstract ApplicationCommandOptionType Type { get; }

    public virtual double? GetMaxValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => null;

    public virtual double? GetMinValue(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => null;

    public virtual int? GetMaxLength(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => null;

    public virtual int? GetMinLength(SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration) => null;

    public virtual IChoicesProvider<TContext>? ChoicesProvider => null;

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public virtual Type? AutocompleteProviderType => null;

    public virtual IEnumerable<ChannelType>? AllowedChannelTypes => null;
}

internal interface ISlashCommandTypeReader
{
}
