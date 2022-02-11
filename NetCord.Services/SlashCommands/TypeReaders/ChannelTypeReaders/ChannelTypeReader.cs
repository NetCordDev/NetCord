namespace NetCord.Services.SlashCommands.TypeReaders.ChannelTypeReaders;

public class ChannelTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Channel;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)context.Interaction.Data.ResolvedData!.Channels![new(value)]);
    }
}