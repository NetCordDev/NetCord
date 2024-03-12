namespace NetCord.Gateway;

public class VerificationField(JsonModels.JsonVerificationField jsonModel) : IJsonModel<JsonModels.JsonVerificationField>
{
    private readonly JsonModels.JsonVerificationField _jsonModel = jsonModel;
    JsonModels.JsonVerificationField IJsonModel<JsonModels.JsonVerificationField>.JsonModel => _jsonModel;
    public VerificationFieldType FieldType => _jsonModel.FieldType;
    public string Label => _jsonModel.Label;
    public bool Required => _jsonModel.Required;
    public bool IsResponse => _jsonModel.Response;
    public IReadOnlyList<string> Values => _jsonModel.Values;
}
