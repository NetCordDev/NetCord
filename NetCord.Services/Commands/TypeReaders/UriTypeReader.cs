﻿namespace NetCord.Services.Commands.TypeReaders;

public class UriTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(Uri.TryCreate(input.ToString(), UriKind.Absolute, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));
}
