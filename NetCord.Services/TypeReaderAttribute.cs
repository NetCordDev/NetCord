namespace NetCord.Services;

[AttributeUsage(AttributeTargets.Parameter)]
public class TypeReaderAttribute : Attribute
{
    public Type TypeReaderType { get; }

    public TypeReaderAttribute(Type typeReaderType)
    {
        TypeReaderType = typeReaderType;
    }
}
