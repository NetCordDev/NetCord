namespace NetCord.Services.Interactions;

[AttributeUsage(AttributeTargets.Parameter)]
public class InteractionParameterAttribute : Attribute
{
    public Type? TypeReaderType { get; init; }
}
