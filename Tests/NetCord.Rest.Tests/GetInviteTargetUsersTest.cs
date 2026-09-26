using System.Net;
using System.Text;

namespace NetCord.Rest.Tests;

[TestClass]
public class GetInviteTargetUsersTest(TestContext context)
{
    private static object[] CreateData(IEnumerable<ulong> userIds, string newLine, bool finalNewLine)
    {
        MemoryStream stream = new();
        using StreamWriter writer = new(stream, Encoding.UTF8, leaveOpen: true);

        writer.Write("user_id");

        foreach (var userId in userIds)
        {
            writer.Write(newLine);
            writer.Write(userId);
        }

        if (finalNewLine)
            writer.Write(newLine);

        writer.Flush();

        stream.Position = 0;
        return [userIds, stream];
    }

    public static IEnumerable<object[]> GetInviteTargetUsersData()
    {
        return from separator in (IEnumerable<string>)["\r\n", "\n"]
               from finalNewLine in (IEnumerable<bool>)[true, false]
               from data in
               (IEnumerable<object[]>)[
                   CreateData(CreateUserIds(0), separator, finalNewLine),
                   CreateData(CreateUserIds(1), separator, finalNewLine),
                   CreateData(CreateUserIds(3), separator, finalNewLine),
                   CreateData(CreateUserIds(100), separator, finalNewLine),
                   CreateData(CreateUserIds(100000), separator, finalNewLine),
                   CreateData(CreateUserIds(1000000), separator, finalNewLine),
               ]
               select data;

        static IEnumerable<ulong> CreateUserIds(int count)
        {
            var baseSnowflake = Snowflake.Create(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), 31, 12, 2332);

            return Enumerable.Range(0, count).Select(i => (ulong)i + baseSnowflake);
        }
    }

    [TestMethod]
    [DynamicData(nameof(GetInviteTargetUsersData))]
    public async Task GetInviteTargetUsersAsync(IEnumerable<ulong> userIds, Stream stream)
    {
        using RestClient client = new(new RestClientConfiguration
        {
            RequestHandler = new GetInviteTargetUsersRequestHandler(stream),
        });

        using var expectedEnumerator = userIds.GetEnumerator();

        await foreach (var userId in client.GetInviteTargetUsersAsync("inviteCode", cancellationToken: context.CancellationToken).ConfigureAwait(false))
        {
            Assert.IsTrue(expectedEnumerator.MoveNext());
            Assert.AreEqual(expectedEnumerator.Current, userId);
        }

        Assert.IsFalse(expectedEnumerator.MoveNext());
    }

    private class GetInviteTargetUsersRequestHandler(Stream responseStream) : IRestRequestHandler
    {
        public void AddDefaultHeader(string name, IEnumerable<string> values)
        {
        }

        public void Dispose()
        {
            responseStream.Dispose();
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StreamContent(responseStream),
            });
        }
    }
}

