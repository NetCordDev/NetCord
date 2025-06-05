namespace NetCord.Hosting;

/// <summary>
/// Represents a factory that creates <see cref="IToken"/> instances.
/// </summary>
public interface ITokenFactory
{
    /// <summary>
    /// Creates a new <see cref="IToken"/> instance from the specified token string.
    /// </summary>
    /// <param name="token">The token string to create the <see cref="IToken"/> instance from.</param>
    /// <returns>A new <see cref="IToken"/> instance created from the specified token string.</returns>
    public IToken CreateToken(string token);
}
