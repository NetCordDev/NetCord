namespace NetCord.Services.ApplicationCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MaxValueAttribute : Attribute
{
    public double MaxValue { get; }

    public MaxValueAttribute(double maxValue)
    {
        MaxValue = maxValue;
    }
}