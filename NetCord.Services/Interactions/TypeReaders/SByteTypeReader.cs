﻿using System.Globalization;

namespace NetCord.Services.Interactions.TypeReaders;

public class SByteTypeReader<TContext> : InteractionTypeReader<TContext> where TContext : IInteractionContext
{
    public override ValueTask<TypeReaderResult> ReadAsync(ReadOnlyMemory<char> input, TContext context, InteractionParameter<TContext> parameter, InteractionServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider) => new(sbyte.TryParse(input.Span, NumberStyles.AllowLeadingSign, configuration.CultureInfo, out var result) ? TypeReaderResult.Success(result) : TypeReaderResult.ParseFail(parameter.Name));
}
