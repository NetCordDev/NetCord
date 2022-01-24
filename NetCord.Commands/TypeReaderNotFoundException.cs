namespace NetCord.Commands;

public class TypeReaderNotFoundException : Exception
{
    internal TypeReaderNotFoundException(string typeName) : base(typeName)
    {
    }
}