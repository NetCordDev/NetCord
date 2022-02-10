namespace NetCord.Services.SlashCommands;

public class SlashCommandModule<TContext> : BaseSlashCommandModule<TContext> where TContext : ISlashCommandContext
{
    public Task RespondAsync(InteractionCallback callback, RequestOptions? options = null) => Context.Interaction.SendResponseAsync(callback, options);
}