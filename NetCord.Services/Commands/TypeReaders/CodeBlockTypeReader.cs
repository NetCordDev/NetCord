﻿namespace NetCord.Services.Commands.TypeReaders;

public class CodeBlockTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(CodeBlock.TryParse(input.Span, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));
}
