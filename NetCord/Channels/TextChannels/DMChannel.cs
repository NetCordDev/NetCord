using NetCord.Rest;

namespace NetCord;

public partial class DMChannel : TextChannel
{
    public IReadOnlyDictionary<ulong, User> Users { get; }

    public DMChannel(JsonModels.JsonChannel jsonModel, RestClient client) : base(jsonModel, client)
    {
        Users = jsonModel.Users.ToDictionaryOrEmpty(u => u.Id, u => new User(u, client));
    }

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
