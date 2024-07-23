namespace NetCord.Rest;

public partial record MessageReactionsPaginationProperties : PaginationProperties<ulong>, IPaginationProperties<ulong, MessageReactionsPaginationProperties>
{
    public ReactionType? Type { get; set; }

    static MessageReactionsPaginationProperties IPaginationProperties<ulong, MessageReactionsPaginationProperties>.Create() => new();
    static MessageReactionsPaginationProperties IPaginationProperties<ulong, MessageReactionsPaginationProperties>.Create(MessageReactionsPaginationProperties properties) => new(properties);
}
