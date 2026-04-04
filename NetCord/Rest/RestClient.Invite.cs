using System.Buffers;
using System.Runtime.CompilerServices;

using NetCord.Gateway;

namespace NetCord.Rest;

public partial class RestClient
{
    [GenerateAlias([typeof(RestInvite)], nameof(RestInvite.Code), TypeNameOverride = nameof(Invite))]
    public async Task<RestInvite> GetGuildInviteAsync(string inviteCode, bool withCounts = false, bool withExpiration = false, ulong? guildScheduledEventId = null, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
    {
        if (guildScheduledEventId.HasValue)
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}&guild_scheduled_event_id={guildScheduledEventId}", null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
        else
            return new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}", $"?with_counts={withCounts}&with_expiration={withExpiration}", null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);
    }

    [GenerateAlias([typeof(RestInvite)], nameof(RestInvite.Code), TypeNameOverride = nameof(Invite))]
    public async Task<RestInvite> DeleteGuildInviteAsync(string inviteCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Delete, $"/invites/{inviteCode}", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonRestInvite).ConfigureAwait(false), this);

    [GenerateAlias([typeof(RestInvite)], nameof(RestInvite.Code), TypeNameOverride = nameof(Invite))]
    public async IAsyncEnumerable<ulong> GetInviteTargetUsersAsync(string inviteCode, RestRequestProperties? properties = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var stream = await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}/target-users", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false);

        var array = ArrayPool<byte>.Shared.Rent(4096);
        var buffer = array.AsMemory();

        try
        {
            int processingIndex;
            int processingEndIndex;

            // Skip header
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);

                if (bytesRead is 0)
                    yield break;

                var span = buffer.Span[..bytesRead];

                processingIndex = span.IndexOf((byte)'\n');

                if (processingIndex >= 0)
                {
                    processingEndIndex = bytesRead;
                    break;
                }
            }

            processingIndex++;

            while (true)
            {
                while (processingIndex < processingEndIndex)
                {
                    var span = buffer.Span[processingIndex..processingEndIndex];

                    int end = span.IndexOf((byte)'\n');

                    if (end < 0)
                        break;

                    var line = span[..end];

                    if (line.EndsWith((byte)'\r'))
                        line = line[..^1];

                    yield return Snowflake.Parse(line);

                    processingIndex += end + 1;
                }

                buffer.Span[processingIndex..processingEndIndex].CopyTo(buffer.Span);

                int bytesRead = await stream.ReadAsync(buffer[(processingEndIndex - processingIndex)..], cancellationToken).ConfigureAwait(false);

                if (bytesRead is 0)
                {
                    var span = buffer.Span[..(processingEndIndex - processingIndex)];
                    if (!span.IsEmpty)
                        yield return Snowflake.Parse(span);

                    break;
                }

                processingEndIndex = bytesRead + processingEndIndex - processingIndex;
                processingIndex = 0;
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(array);
            await stream.DisposeAsync().ConfigureAwait(false);
        }
    }

    public async Task UpdateInviteTargetUsersAsync(string inviteCode, InviteTargetUsersProperties inviteTargetUsersProperties, RestRequestProperties? requestProperties = null, CancellationToken cancellationToken = default)
    {
        using (HttpContent content = new MultipartFormDataContent()
        {
            { inviteTargetUsersProperties.Serialize(), "target_users_file", "target_users_file" }
        })
            await SendRequestAsync(HttpMethod.Put, content, $"/invites/{inviteCode}/target-users", null, null, requestProperties, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async Task<InviteTargetUsersJobStatus> GetInviteTargetUsersJobStatusAsync(string inviteCode, RestRequestProperties? properties = null, CancellationToken cancellationToken = default)
        => new(await (await SendRequestAsync(HttpMethod.Get, $"/invites/{inviteCode}/target-users/job-status", null, null, properties, cancellationToken: cancellationToken).ConfigureAwait(false)).ToObjectAsync(Serialization.Default.JsonInviteTargetUsersJobStatus).ConfigureAwait(false));
}
