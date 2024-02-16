namespace NetCord;

public class GuildChannelMention(JsonModels.JsonGuildChannelMention jsonModel) : Entity
{
    public override ulong Id => jsonModel.Id;
    public ulong GuildId => jsonModel.GuildId;
    public ChannelType Type => jsonModel.Type;
    public string Name => jsonModel.Name;
}
