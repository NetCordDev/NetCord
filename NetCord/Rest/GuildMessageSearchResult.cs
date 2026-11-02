using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public abstract class GuildMessageSearchResult
{
    public sealed class Success(RestMessage message, GuildMessagesSearchResultData data) : GuildMessageSearchResult
    {
        /// <summary>
        /// The message that was found.
        /// </summary>
        public RestMessage Message => message;

        /// <summary>
        /// The data associated with the search result.
        /// </summary>
        public GuildMessagesSearchResultData Data => data;
    }

    public sealed class Indexing(JsonGuildMessagesSearchResult jsonModel) : GuildMessageSearchResult
    {
        /// <summary>
        /// The message of the response.
        /// </summary>
        public string Message { get; } = jsonModel.Message!;

        /// <summary>
        /// The code of the response.
        /// </summary>
        public int Code { get; } = jsonModel.Code.GetValueOrDefault();

        /// <summary>
        /// The number of documents indexed so far.
        /// </summary>
        public int DocumentsIndexed { get; } = jsonModel.DocumentsIndexed.GetValueOrDefault();

        /// <summary>
        /// The number of seconds to wait before retrying the request.
        /// </summary>
        public int RetryAfter { get; } = jsonModel.RetryAfter.GetValueOrDefault();
    }
}

public class GuildMessagesSearchResultData(JsonGuildMessagesSearchResult jsonModel, RestClient client)
{
    /// <summary>
    /// Whether the server is doing a deep historical index.
    /// </summary>
    public bool DoingDeepHistoricalIndex { get; } = jsonModel.DoingDeepHistoricalIndex.GetValueOrDefault();

    /// <summary>
    /// The total number of results for the search.
    /// </summary>
    public int TotalResults { get; } = jsonModel.TotalResults.GetValueOrDefault();

    /// <summary>
    /// The threads associated with the search results.
    /// </summary>
    public IReadOnlyList<GuildThread> Threads { get; } = [.. jsonModel.Threads.SelectOrEmpty(t => GuildThread.CreateFromJson(t, client))];

    /// <summary>
    /// The thread users associated with the search results.
    /// </summary>
    public IReadOnlyList<ThreadUser> ThreadUsers { get; } = [.. jsonModel.ThreadUsers.SelectOrEmpty(t => new ThreadUser(t, client))];
}
