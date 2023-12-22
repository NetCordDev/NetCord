namespace NetCord.Rest;

public partial class UserCommandProperties : ApplicationCommandProperties
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name">Name of the command (1-32 characters).</param>
    public UserCommandProperties(string name) : base(ApplicationCommandType.User, name)
    {
    }
}
