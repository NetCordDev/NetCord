namespace NetCord.Services.ApplicationCommands.TypeReaders;

public class AttachmentTypeReader<TContext> : SlashCommandTypeReader<TContext> where TContext : IApplicationCommandContext
{
    public override ApplicationCommandOptionType Type => ApplicationCommandOptionType.Attachment;

    public override ValueTask<TypeReaderResult> ReadAsync(string value, TContext context, SlashCommandParameter<TContext> parameter, ApplicationCommandServiceConfiguration<TContext> configuration, IServiceProvider? serviceProvider)
    {
        return new(TypeReaderResult.Success(((SlashCommandInteraction)context.Interaction).Data.ResolvedData!.Attachments![Snowflake.Parse(value)]));
    }
}
