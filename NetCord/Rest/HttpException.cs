using System.Text.Json;

namespace NetCord;

public class HttpException : Exception
{
    public JsonDocument RawValue { get; }
    public int Code => RawValue.RootElement.GetProperty("code").GetInt32();

    internal HttpException(JsonDocument d) : base(d.RootElement.GetProperty("message").GetString())
    {
        RawValue = d;
    }
}