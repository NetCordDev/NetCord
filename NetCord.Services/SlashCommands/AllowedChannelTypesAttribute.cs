namespace NetCord.Services.SlashCommands;

[AttributeUsage(AttributeTargets.Parameter)]
public class AllowedChannelTypesAttribute : Attribute
{
    public IEnumerable<ChannelType>? ChannelTypes { get; }

    public AllowedChannelTypesAttribute(ChannelType[]? channelTypes)
    {
        ChannelTypes = channelTypes;
    }
}