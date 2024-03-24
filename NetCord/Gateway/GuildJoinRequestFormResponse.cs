namespace NetCord.Gateway;

public class GuildJoinRequestFormResponse(JsonModels.JsonGuildJoinRequestFormResponse jsonModel) : IJsonModel<JsonModels.JsonGuildJoinRequestFormResponse>
{
    JsonModels.JsonGuildJoinRequestFormResponse IJsonModel<JsonModels.JsonGuildJoinRequestFormResponse>.JsonModel => jsonModel;

    public GuildJoinRequestFormResponseFieldType FieldType => jsonModel.FieldType;
    public string Label => jsonModel.Label;
    public bool Required => jsonModel.Required;
    public bool IsResponse => jsonModel.Response;
    public IReadOnlyList<string> Values => jsonModel.Values;
}
