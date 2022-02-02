﻿namespace NetCord.Services.SlashCommands.TypeReaders;

public class Int64TypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : BaseSlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Integer;

    public override Task<object> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options) => Task.FromResult((object)long.Parse(value, System.Globalization.CultureInfo.InvariantCulture));

    public override double? GetMaxValue(SlashCommandParameter<TContext> parameter) => long.MaxValue;

    public override double? GetMinValue(SlashCommandParameter<TContext> parameter) => long.MinValue;
}