﻿namespace NetCord.Services.Commands;

public abstract class CommandTypeReader<TContext> : ICommandTypeReader where TContext : ICommandContext
{
    public abstract Task<object> ReadAsync(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options);
}

internal interface ICommandTypeReader
{
}

//public delegate Task<object> TypeReaderDelegate<TContext>(string input, TContext context, CommandParameter<TContext> parameter, CommandServiceOptions<TContext> options) where TContext : ICommandContext;