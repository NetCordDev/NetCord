namespace NetCord.Gateway;

public class VerificationField(JsonModels.JsonVerificationField jsonModel) : IJsonModel<JsonModels.JsonVerificationField>
{
    JsonModels.JsonVerificationField IJsonModel<JsonModels.JsonVerificationField>.JsonModel => jsonModel;
    
    public VerificationFieldType FieldType => jsonModel.FieldType;
    public string Label => jsonModel.Label;
    public bool Required => jsonModel.Required;
    public bool IsResponse => jsonModel.Response;
    public IReadOnlyList<string> Values => jsonModel.Values;
}
