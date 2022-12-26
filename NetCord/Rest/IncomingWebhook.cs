using NetCord.JsonModels;

namespace NetCord.Rest;

public class IncomingWebhook : Webhook
{
    public IncomingWebhook(JsonWebhook jsonModel, RestClient client) : base(jsonModel, client)
    {
    }

    public string Token => _jsonModel.Token!;

    #region Webhook
    public Task<Webhook> ModifyWithTokenAsync(Action<WebhookOptions> action, RequestProperties? properties = null) => _client.ModifyWebhookWithTokenAsync(Id, Token, action, properties);
    public Task DeleteWithTokenAsync(RequestProperties? properties = null) => _client.DeleteWebhookWithTokenAsync(Id, Token, properties);
    public Task<RestMessage?> ExecuteAsync(WebhookMessageProperties messageProperties, bool wait = false, ulong? threadId = null, RequestProperties? properties = null) => _client.ExecuteWebhookAsync(Id, Token, messageProperties, wait, threadId, properties);
    public Task<RestMessage> GetMessageAsync(ulong messageId, RequestProperties? properties = null) => _client.GetWebhookMessageAsync(Id, Token, messageId, properties);
    public Task<RestMessage> ModifyMessageAsync(ulong messageId, Action<MessageOptions> action, ulong? threadId = null, RequestProperties? properties = null) => _client.ModifyWebhookMessageAsync(Id, Token, messageId, action, threadId, properties);
    public Task DeleteMessageAsync(ulong messageId, ulong? threadId = null, RequestProperties? properties = null) => _client.DeleteWebhookMessageAsync(Id, Token, messageId, threadId, properties);
    #endregion
}
