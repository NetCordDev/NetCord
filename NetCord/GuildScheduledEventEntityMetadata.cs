namespace NetCord;

public class GuildScheduledEventEntityMetadata(JsonModels.JsonGuildScheduledEventEntityMetadata jsonModel)
{
    public string? Location { get; } = jsonModel.Location;
}

