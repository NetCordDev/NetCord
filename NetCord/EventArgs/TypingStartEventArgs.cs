namespace NetCord;

public class TypingStartEventArgs : IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>
{
    JsonModels.EventArgs.JsonTypingStartEventArgs IJsonModel<JsonModels.EventArgs.JsonTypingStartEventArgs>.JsonModel => _jsonModel;
    private readonly JsonModels.EventArgs.JsonTypingStartEventArgs _jsonModel;

    public TypingStartEventArgs(JsonModels.EventArgs.JsonTypingStartEventArgs jsonModel, RestClient client)
    {
        _jsonModel = jsonModel;
        if (jsonModel.User != null)
            User = new(jsonModel.User, _jsonModel.GuildId.GetValueOrDefault(), client);
    }

    public Snowflake ChannelId => _jsonModel.ChannelId;

    public Snowflake? GuildId => _jsonModel.GuildId;

    public Snowflake UserId => _jsonModel.UserId;

    public DateTimeOffset Timestamp => _jsonModel.Timestamp;

    public GuildUser? User { get; }
}
