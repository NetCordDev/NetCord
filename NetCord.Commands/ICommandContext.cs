namespace NetCord.Commands;

public interface ICommandContext
{
    public UserMessage Message { get; }
    public Guild? Guild { get; }
}