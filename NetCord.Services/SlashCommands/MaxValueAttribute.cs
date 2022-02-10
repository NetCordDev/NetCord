namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MaxValueAttribute : Attribute
{
    public double MaxValue { get; }

    public MaxValueAttribute(double minValue)
    {
        MaxValue = minValue;
    }
}