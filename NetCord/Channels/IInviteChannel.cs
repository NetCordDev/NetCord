namespace NetCord;

public interface IInviteChannel : IChannel
{
    string? Name { get; }
    string? Icon { get; }
    IReadOnlyList<string>? RecipientUsernames { get; }
}
