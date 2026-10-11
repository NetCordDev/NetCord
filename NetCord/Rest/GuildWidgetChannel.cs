using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public class GuildWidgetChannel(JsonGuildWidgetChannel jsonModel) : Entity(jsonModel)
{
    public string Name { get; } = jsonModel.Name;

    public int Position { get; } = jsonModel.Position;
}
