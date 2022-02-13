namespace NetCord.Services.SlashCommands.TypeReaders;

public class AttachmentTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : ISlashCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Attachment;

    public override Task<object?> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, SlashCommandServiceOptions<TContext> options)
    {
        return Task.FromResult((object?)context.Interaction.Data.ResolvedData!.Attachments![new(value)]);
    }
}