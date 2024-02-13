using System.Globalization;
using System.Web;

namespace NetCord;

public class AttachmentExpirationInfo
{
    public AttachmentExpirationInfo(string url)
    {
        var query = HttpUtility.ParseQueryString(new Uri(url).Query);
        ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["ex"]!, NumberStyles.AllowHexSpecifier));
        IssuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["is"]!, NumberStyles.AllowHexSpecifier));
        Signature = query["hm"]!;
    }

    public DateTimeOffset ExpiresAt { get; }

    public DateTimeOffset IssuedAt { get; }

    public string Signature { get; }
}
