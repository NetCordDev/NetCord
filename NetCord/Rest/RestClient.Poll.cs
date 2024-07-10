using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage), typeof(IPartialMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = nameof(Message))]
    public IAsyncEnumerable<User> GetMessagePollAnswerVotersAsync(ulong channelId, ulong messageId, int answerId, PaginationProperties<ulong>? paginationProperties = null, RestRequestProperties? properties = null)
    {
        paginationProperties = PaginationProperties<ulong>.PrepareWithDirectionValidation(paginationProperties, PaginationDirection.After, 100);

        return new QueryPaginationAsyncEnumerable<User, ulong>(
            this,
            paginationProperties,
            async s => (await s.ToObjectAsync(Serialization.Default.JsonMessagePollAnswerVotersResult).ConfigureAwait(false)).Users.Select(u => new User(u, this)),
            u => u.Id,
            HttpMethod.Get,
            $"/channels/{channelId}/polls/{messageId}/answers/{answerId}",
            new(paginationProperties.Limit.GetValueOrDefault(), paginationProperties.Direction.GetValueOrDefault(), id => id.ToString()),
            new(channelId),
            properties);
    }

    [GenerateAlias([typeof(TextChannel)], nameof(TextChannel.Id))]
    [GenerateAlias([typeof(RestMessage), typeof(IPartialMessage)], nameof(RestMessage.ChannelId), nameof(RestMessage.Id), TypeNameOverride = nameof(Message))]
    public async Task<RestMessage> EndMessagePollAsync(ulong channelId, ulong messageId, RestRequestProperties? properties = null)
        => new(await (await SendRequestAsync(HttpMethod.Post, $"/channels/{channelId}/polls/{messageId}/expire", null, new(channelId), properties).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonMessage).ConfigureAwait(false), this);
}
