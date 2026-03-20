using System.Diagnostics.CodeAnalysis;

namespace NetCord.Services.ApplicationCommands;

public abstract class SlashCommandTypeReader<TContext> : ISlashCommandTypeReader where TContext : IApplicationCommandContext
{
    public abstract ValueTask<SlashCommandTypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);

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

public abstract class SlashCommandTypeReaderResult : IExecutionResult
{
    public static SlashCommandTypeReaderResult Success(object? value) => new SlashCommandTypeReaderSuccessResult(value);

    public static SlashCommandTypeReaderResult Fail(string message) => new SlashCommandTypeReaderFailResult(message);

    public static SlashCommandTypeReaderResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");
}

public class SlashCommandTypeReaderSuccessResult(object? value) : SlashCommandTypeReaderResult
{
    public object? Value { get; } = value;
}

public class SlashCommandTypeReaderFailResult(string message) : SlashCommandTypeReaderResult, IFailResult
{
    public string Message { get; } = message;
}

public class SlashCommandTypeReaderExceptionResult(Exception exception) : SlashCommandTypeReaderFailResult(exception.Message)
{
    public Exception Exception { get; } = exception;
}
