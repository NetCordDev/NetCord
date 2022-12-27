using NetCord;

namespace TokenTest;

[TestClass]
public class Test
{
    [TestMethod]
    public void TokenTest()
    {
        var tokenId = 803377270878109726uL;
        var tokenType = TokenType.Bot;
        var rawToken = "ODAzMzc3MjcwODc4MTA5NzI2.GAVT0D.DDNY-77JFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";
        Token token = new(tokenType, rawToken);

        Assert.AreEqual(tokenType, token.Type);
        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual(tokenId, token.Id);
        Assert.AreEqual(rawToken, token.ToString());
        Assert.AreEqual($"Bot {rawToken}", token.ToHttpHeader());

        tokenType = TokenType.Bearer;
        token = new(tokenType, rawToken);
        Assert.AreEqual(tokenType, token.Type);
        Assert.AreEqual(rawToken, token.RawToken);
        Assert.AreEqual(tokenId, token.Id);
        Assert.AreEqual(rawToken, token.ToString());
        Assert.AreEqual($"Bearer {rawToken}", token.ToHttpHeader());

        tokenType = TokenType.Bot;
        rawToken = "ODAzMzc3MjcwOFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";
        token = new(tokenType, rawToken);

        Assert.AreEqual(tokenType, token.Type);
        Assert.AreEqual(rawToken, token.RawToken);
        Assert.ThrowsException<InvalidOperationException>(() => token.Id);
        Assert.AreEqual(rawToken, token.ToString());
        Assert.AreEqual($"Bot {rawToken}", token.ToHttpHeader());

        rawToken = "ODAzMzc3MjcwODc4MTA5Nz2s.GAVT0D.DDNY-77JFnrMDZxSSwlq3WdlZH-3grIPBPKrSA";
        token = new(tokenType, rawToken);

        Assert.AreEqual(tokenType, token.Type);
        Assert.AreEqual(rawToken, token.RawToken);
        Assert.ThrowsException<InvalidOperationException>(() => token.Id);
        Assert.AreEqual(rawToken, token.ToString());
        Assert.AreEqual($"Bot {rawToken}", token.ToHttpHeader());

        Assert.ThrowsException<ArgumentException>(() => new Token(tokenType, null!));
        Assert.ThrowsException<ArgumentException>(() => new Token(tokenType, string.Empty));
    }
}
