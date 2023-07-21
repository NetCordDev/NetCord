namespace NetCord;

[Flags]
public enum RoleFlags
{
    /// <summary>
    /// Role can be selected by members in an onboarding prompt.
    /// </summary>
    InPrompt = 1 << 0,
}
