namespace NetCord.Services;

public class TypeReaderNotFoundException : Exception
{
    internal TypeReaderNotFoundException(string typeName) : base(typeName)
    {
    }
}
