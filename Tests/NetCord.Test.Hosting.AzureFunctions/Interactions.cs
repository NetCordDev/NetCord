using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using NetCord.Hosting.AzureFunctions;

namespace NetCord.Test.Hosting.AzureFunctions;

public class Interactions(DiscordInteractionsHandler handler)
{
    [Function("interactions")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req, FunctionContext context)
    {
        var res = await handler.HandleRequestAsync(req, context).ConfigureAwait(false);
        return res;
    }
}
