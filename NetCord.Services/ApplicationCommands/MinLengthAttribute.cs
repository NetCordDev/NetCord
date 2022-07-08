namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MinLengthAttribute : Attribute
{
    public int MinLength { get; }

    public MinLengthAttribute(int minLength)
    {
        MinLength = minLength;
    }
}