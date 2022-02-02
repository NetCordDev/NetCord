namespace NetCord.Services.Commands;

public interface ICommandContext : IContext
{
    public Message Message { get; }
}