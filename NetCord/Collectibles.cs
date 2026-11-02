using NetCord.JsonModels;

namespace NetCord;

public class Collectibles(JsonCollectibles jsonModel)
{
    /// <summary>
    /// The nameplate the user has.
    /// </summary>
    public Nameplate? Nameplate { get; } = jsonModel.Nameplate is { } nameplate ? new(nameplate) : null;
}
