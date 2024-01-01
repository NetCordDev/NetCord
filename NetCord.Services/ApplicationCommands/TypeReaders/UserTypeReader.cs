﻿namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class UserTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.User;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TypeReaderResult.Success(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Users![Snowflake.Parse(value)]));
    }
}
