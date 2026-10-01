using NetCord.JsonModels;
using NetCord.Rest;

namespace NetCord;

public class MessageInteraction(JsonMessageInteraction jsonModel, ulong? guildId, RestClient client) : Entity
{
    public override ulong Id { get; } = jsonModel.Id;

    public InteractionType Type { get; } = jsonModel.Type;

    public string Name { get; } = jsonModel.Name;

    public User User { get; } = CreateUser(jsonModel, guildId, client);

    private static User CreateUser(JsonMessageInteraction jsonModel, ulong? guildId, RestClient client)
    {
        if (jsonModel.GuildUser is { } guildUser)
        {
            guildUser.User = jsonModel.User;

            return new GuildUser(guildUser, guildId.GetValueOrDefault(), client);
        }

        return new(jsonModel.User, client);
    }
}
