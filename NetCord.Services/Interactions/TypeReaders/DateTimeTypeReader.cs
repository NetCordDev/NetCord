﻿namespace NetCord.Services.Interactions.TypeReaders;

public class DateTimeTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(DateTime.Parse(input.Span, configuration.CultureInfo));
}
