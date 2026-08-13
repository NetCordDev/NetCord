namespace NetCord.Hosting;

public interface IHttpInteractionHandler
{
    /// <summary>
    /// Handles HTTP <see cref="Interaction"/>s.
    /// </summary>
    /// <param name="interaction">The <see cref="Interaction"/> received.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    public ValueTask HandleAsync(Interaction interaction);
}
