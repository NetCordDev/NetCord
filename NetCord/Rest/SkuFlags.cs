namespace NetCord.Rest;

[Flags]
public enum SkuFlags
{
    GuildSubscription = 1 << 7,
    UserSubscription = 1 << 8,
}
