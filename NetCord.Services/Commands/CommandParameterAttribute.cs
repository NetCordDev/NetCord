namespace NetCord.Services.Commands;

[AttributeUsage(AttributeTargets.Parameter)]
public class CommandParameterAttribute : Attribute
{
    public bool Remainder { get; init; }
    public Type? TypeReaderType { get; init; }
}
