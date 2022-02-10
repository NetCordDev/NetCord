namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MinValueAttribute : Attribute
{
    public double MinValue { get; }

    public MinValueAttribute(double minValue)
    {
        MinValue = minValue;
    }
}