using System.Globalization;
using System.Web;

namespace NetCord;

/// <summary>
/// Contains information on a CDN URL's issue and expiry.
/// </summary>
public class AttachmentExpirationInfo
{
    public AttachmentExpirationInfo(string url)
    {
        var query = HttpUtility.ParseQueryString(new Uri(url).Query);
        ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["ex"]!, NumberStyles.AllowHexSpecifier));
        IssuedAt = DateTimeOffset.FromUnixTimeSeconds(long.Parse(query["is"]!, NumberStyles.AllowHexSpecifier));
        Signature = query["hm"]!;
    }

    /// <summary>
    /// A timestamp indicating when the CDN URL will expire.
    /// </summary>
    public DateTimeOffset ExpiresAt { get; }

    /// <summary>
    /// A timestamp indicating when the CDN URL was issued.
    /// </summary>
    public DateTimeOffset IssuedAt { get; }

    /// <summary>
    /// A unique signature, valid until the CDN URL's expiration.
    /// </summary>
    public string Signature { get; }
}
