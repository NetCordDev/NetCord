namespace NetCord.Services.Commands;

public abstract class CommandTypeReader<TContext> : ICommandTypeReader where TContext : ICommandContext
{
    public abstract ValueTask<CommandTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

public abstract class CommandTypeReaderResult : IExecutionResult
{
    public static CommandTypeReaderResult Success(object? value, int read) => new CommandTypeReaderSuccessResult(value, read);

    public static CommandTypeReaderResult Fail(string message) => new CommandTypeReaderFailResult(message);

    public static CommandTypeReaderResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");
}

public class CommandTypeReaderSuccessResult(object? value, int read) : CommandTypeReaderResult
{
    public object? Value { get; } = value;

    public int Read { get; } = read;
}

public class CommandTypeReaderFailResult(string message) : CommandTypeReaderResult, IFailResult
{
    public string Message { get; } = message;
}

public class CommandTypeReaderExceptionResult(Exception exception) : CommandTypeReaderFailResult(exception.Message)
{
    public Exception Exception { get; } = exception;
}

public abstract class CommandTypeParser<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public sealed override async ValueTask<CommandTypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        ReadOnlyMemory<char> parseInput;
        if (parameter.Remainder)
            parseInput = input;
        else
        {
            int index = input.Span.IndexOfAny(configuration.ParameterSeparatorsSearchValues);
            parseInput = index >= 0 ? input[..index] : input;
        }

        var result = await ParseAsync(parseInput, context, parameter, configuration, serviceProvider).ConfigureAwait(false);

        return result.ToTypeReaderResult(parseInput.Length);
    }

    public abstract ValueTask<CommandTypeParserResult> ParseAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider);
}

internal interface ICommandTypeReader
{
}

public abstract class CommandTypeParserResult : IExecutionResult
{
    public static CommandTypeParserResult Success(object? value) => new CommandTypeParserSuccessResult(value);

    public static CommandTypeParserResult Fail(string message) => new CommandTypeParserFailResult(message);

    public static CommandTypeParserResult ParseFail(string parameterName) => Fail($"Failed to parse '{parameterName}'.");

    internal abstract CommandTypeReaderResult ToTypeReaderResult(int read);
}

public class CommandTypeParserSuccessResult(object? value) : CommandTypeParserResult
{
    public object? Value => value;

    internal sealed override CommandTypeReaderResult ToTypeReaderResult(int read) => CommandTypeReaderResult.Success(value, read);
}

public class CommandTypeParserFailResult(string message) : CommandTypeParserResult, IFailResult
{
    public string Message => message;

    internal sealed override CommandTypeReaderResult ToTypeReaderResult(int read) => CommandTypeReaderResult.Fail(message);
}

public class CommandTypeParserExceptionResult(Exception exception) : CommandTypeParserFailResult(exception.Message)
{
    public Exception Exception { get; } = exception;
}
