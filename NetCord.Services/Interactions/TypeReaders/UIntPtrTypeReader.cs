﻿using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class UIntPtrTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override Task<object?> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => Task.FromResult<object?>(nuint.Parse(input.Span, NumberStyles.None, configuration.CultureInfo));
}
