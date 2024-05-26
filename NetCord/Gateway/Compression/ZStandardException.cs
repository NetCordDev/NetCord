namespace NetCord.Gateway.Compression;

public class ZStandardException(nuint code) : Exception($"ZStandard returned an '{ZStandard.GetErrorName(code)}' error.")
{
}
