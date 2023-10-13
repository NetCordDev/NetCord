﻿namespace NetCord.Services.Commands.TypeReaders;

public class BooleanTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, CommandParameter<TContext> parameter, CommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(bool.Parse(input.Span));
}
