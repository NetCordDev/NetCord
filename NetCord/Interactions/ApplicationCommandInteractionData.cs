using System.Collections.ObjectModel;

namespace NetCord;

public class ApplicationCommandInteractionData : InteractionData, IEntity
{
    public DiscordId Id => _jsonEntity.Id.GetValueOrDefault();

    public string Name => _jsonEntity.Name!;

    public ApplicationCommandType Type => _jsonEntity.Type.GetValueOrDefault();

    public ApplicationCommandInteractionResolvedData? ResolvedData { get; }

    public ReadOnlyCollection<ApplicationCommandInteractionDataOption> Options { get; }

    internal ApplicationCommandInteractionData(JsonModels.JsonInteractionData jsonEntity, DiscordId? guildId, RestClient client) : base(jsonEntity)
    {
        if (jsonEntity.ResolvedData != null)
            ResolvedData = new(jsonEntity.ResolvedData, guildId, client);
        Options = new(jsonEntity.Options.SelectOrEmpty(o => new ApplicationCommandInteractionDataOption(o)).ToList());
    }
}