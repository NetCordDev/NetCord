namespace NetCord;

public interface IDMChannel : ITextChannel, IPinnableChannel
{
    ulong? RecipientId { get; }
}
