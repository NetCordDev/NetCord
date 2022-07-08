namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MaxLengthAttribute : Attribute
{
    public int MaxLength { get; }

    public MaxLengthAttribute(int maxLength)
    {
        MaxLength = maxLength;
    }
}