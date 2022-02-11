using System.Numerics;

namespace NetCord.Services.Commands.TypeReaders;

public class BigIntegerTypeReader<TContext> : CommandTypeReader<TContext> where TContext : ICommandContext
{
    public override Task<object?> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) => Task.FromResult((object?)BigInteger.Parse(input, options.CultureInfo));
}