namespace NetCord.Rest;

public class GuildApplicationCommand : ApplicationCommand
{
    public Snowflake GuildId => _jsonModel.GuildId.GetValueOrDefault();

    public GuildApplicationCommand(JsonModels.JsonApplicationCommand jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    #region Interactions.ApplicationCommands
    public override async Task<ApplicationCommand> ModifyAsync(Action<ApplicationCommandOptions> action, RequestProperties? properties = null) => await _client.ModifyGuildApplicationCommandAsync(ApplicationId, GuildId, Id, action, properties).ConfigureAwait(false);
    public override Task DeleteAsync(RequestProperties? properties = null) => _client.DeleteGuildApplicationCommandAsync(ApplicationId, GuildId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> GetPermissionsAsync(RequestProperties? properties = null) => _client.GetApplicationCommandGuildPermissionsAsync(ApplicationId, GuildId, Id, properties);
    public Task<ApplicationCommandGuildPermissions> OverwritePermissionsAsync(IEnumerable<ApplicationCommandGuildPermissionProperties> newPermissions, RequestProperties? properties = null) => _client.OverwriteApplicationCommandGuildPermissionsAsync(ApplicationId, GuildId, Id, newPermissions, properties);
    #endregion
}