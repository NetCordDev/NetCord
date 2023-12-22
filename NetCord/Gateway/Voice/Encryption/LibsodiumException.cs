namespace NetCord.Gateway.Voice.Encryption;

public class LibsodiumException : Exception
{
    public LibsodiumException() : base("Libsodium returned an error.")
    {
    }
}
