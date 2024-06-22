namespace NetCord.Gateway.Compression;

public class ZstandardException(nuint code) : Exception($"Zstandard returned an '{Zstandard.GetErrorName(code)}' error.")
{
}
