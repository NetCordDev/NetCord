namespace NetCord.Services;

public interface IUserContext : IContext
{
    public User User { get; }
}