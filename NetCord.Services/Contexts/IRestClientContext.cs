using NetCord.Rest;

namespace NetCord.Services;

/// <summary>
/// Provides access to the <see cref="RestClient"/> associated with the handled command or interaction.
/// </summary>
public interface IRestClientContext : IContext
{
    /// <summary>
    /// The <see cref="RestClient"/> associated with the handled command or interaction.
    /// </summary>
    public RestClient Client { get; }
}
