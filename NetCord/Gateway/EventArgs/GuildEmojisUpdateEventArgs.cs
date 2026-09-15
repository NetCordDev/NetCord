using NetCord.Rest;

namespace NetCord.Gateway;

/// <summary>
/// Represents the event arguments for a guild emojis update event.
/// </summary>
public class GuildEmojisUpdateEventArgs(JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs jsonModel, RestClient client, IDictionaryProvider dictionaryProvider) 
    : IJsonModel<JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs>
{
    JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs IJsonModel<JsonModels.EventArgs.JsonGuildEmojisUpdateEventArgs>.JsonModel => jsonModel;

    /// <summary>
    /// The ID of the guild where the emojis were updated.
    /// </summary>
    public ulong GuildId => jsonModel.GuildId;

    /// <summary>
    /// The updated dictionary of guild emojis, indexed by their unique identifiers.
    /// </summary>
    public IReadOnlyDictionary<ulong, GuildEmoji> Emojis { get; } = dictionaryProvider.CreateDictionary(
        jsonModel.Emojis, 
        e => e.Id.GetValueOrDefault(), 
        e => new GuildEmoji(e, jsonModel.GuildId, client));
}
