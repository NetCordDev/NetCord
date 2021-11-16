namespace NetCord
{
    public enum VerificationLevel
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
        VeryHigh = 4,
    }

    public enum DefaultMessageNotificationLevel
    {
        AllMessages = 0,
        OnlyMentions = 1,
    }

    public enum ContentFilter
    {
        Disabled = 0,
        MembersWithoutRoles = 1,
        AllMembers = 2,
    }
    public enum MFALevel
    {
        None = 0,
        Elevated = 1,
    }
}
