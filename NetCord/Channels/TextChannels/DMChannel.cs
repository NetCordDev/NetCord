using NetCord.Rest;

namespace NetCord;

public partial class DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : TextChannel(jsonModel, client)
{
    public IReadOnlyDictionary<ulong, User> Users { get; } = jsonModel.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));

    public static new DMChannel CreateFromJson(JsonModels.JsonChannel jsonModel, RestClient client)
    {
        return jsonModel.Type switch
        {
            ChannelType.DMChannel => new DMChannel(jsonModel, client),
            ChannelType.GroupDMChannel => new GroupDMChannel(jsonModel, client),
            _ => new UnknownDMChannel(jsonModel, client),
        };
    }
}
