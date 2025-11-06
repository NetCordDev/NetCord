using NetCord.JsonModels;

namespace NetCord;

public class Collectibles : IJsonModel<JsonCollectibles>
{
    JsonCollectibles IJsonModel<JsonCollectibles>.JsonModel => _jsonModel;
    private readonly JsonCollectibles _jsonModel;

    public Collectibles(JsonCollectibles jsonModel)
    {
        _jsonModel = jsonModel;

        if (jsonModel.Nameplate is { } nameplate)
            Nameplate = new(nameplate);
    }

    /// <summary>
    /// The nameplate the user has.
    /// </summary>
    public Nameplate? Nameplate { get; }
}
