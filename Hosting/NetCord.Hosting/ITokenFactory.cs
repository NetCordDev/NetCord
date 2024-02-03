namespace NetCord.Hosting;

public interface ITokenFactory
{
    public IToken CreateToken(string token);
}
