namespace NetCord.Services;

public class TypeReaderNotFoundException : Exception
{
    public TypeReaderNotFoundException(Type type) : base($"Type reader was not found for '{type}'.")
    {
    }

    public TypeReaderNotFoundException(Type type, Type type2) : base($"Type reader was not found for '{type}' or '{type2}'.")
    {
    }
}
