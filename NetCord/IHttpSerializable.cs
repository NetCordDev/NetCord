namespace NetCord;

public interface IHttpSerializable
{
    /// <summary>
    /// Serializes the object or its part into <see cref="HttpContent"/>.
    /// </summary>
    public HttpContent Serialize();
}
