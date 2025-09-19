using NetCord.Rest.JsonModels;

namespace NetCord.Rest;

public abstract class GuildMessageSearchResult
{
    private GuildMessageSearchResult()
    {
    }

    public sealed class Success(RestMessage message, GuildMessagesSearchResultData data) : GuildMessageSearchResult
    {
        public RestMessage Message => message;
        public GuildMessagesSearchResultData Data => data;
    }

    public sealed class Indexing(JsonGuildMessagesSearchResult jsonModel) : GuildMessageSearchResult, IJsonModel<JsonGuildMessagesSearchResult>
    {
        JsonGuildMessagesSearchResult IJsonModel<JsonGuildMessagesSearchResult>.JsonModel => jsonModel;

        public string Message => jsonModel.Message!;
        public int Code => jsonModel.Code.GetValueOrDefault();
        public int DocumentsIndexed => jsonModel.DocumentsIndexed.GetValueOrDefault();
        public int RetryAfter => jsonModel.RetryAfter.GetValueOrDefault();
    }
}

public class GuildMessagesSearchResultData(JsonGuildMessagesSearchResult jsonModel, RestClient client) : IJsonModel<JsonGuildMessagesSearchResult>
{
    JsonGuildMessagesSearchResult IJsonModel<JsonGuildMessagesSearchResult>.JsonModel => jsonModel;

    public string AnalyticsId => jsonModel.AnalyticsId!;

    public bool DoingDeepHistoricalIndex => jsonModel.DoingDeepHistoricalIndex.GetValueOrDefault();

    public int TotalResults => jsonModel.TotalResults.GetValueOrDefault();

    public IReadOnlyList<GuildThread> Threads { get; } = jsonModel.Threads.SelectOrEmpty(t => GuildThread.CreateFromJson(t, client)).ToArray();

    public IReadOnlyList<ThreadUser> ThreadUsers { get; } = jsonModel.ThreadUsers.SelectOrEmpty(t => new ThreadUser(t, client)).ToArray();
}
