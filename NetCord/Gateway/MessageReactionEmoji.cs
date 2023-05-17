namespace NetCord.Gateway;

public class MessageReactionEmoji : Entity, IJsonModel<NetCord.JsonModels.JsonEmoji>
{
    NetCord.JsonModels.JsonEmoji IJsonModel<NetCord.JsonModels.JsonEmoji>.JsonModel => _jsonModel;
    private readonly NetCord.JsonModels.JsonEmoji _jsonModel;

    public MessageReactionEmoji(NetCord.JsonModels.JsonEmoji jsonModel)
    {
        _jsonModel = jsonModel;
    }

    public override ulong Id
    {
        get
        {
            if (IsStandard)
                throw new InvalidOperationException("This emoji has no id.");
            return _jsonModel.Id.GetValueOrDefault();
        }
    }

    public bool IsStandard => !_jsonModel.Id.HasValue;

    public string? Name => _jsonModel.Name;

    public bool Animated => _jsonModel.Animated;
}
