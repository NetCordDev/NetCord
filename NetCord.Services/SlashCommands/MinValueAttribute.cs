namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class MinValueAttribute : Attribute
{
    public decimal MinValue { get; }

    public MinValueAttribute(decimal minValue)
    {
        MinValue = minValue;
    }
}