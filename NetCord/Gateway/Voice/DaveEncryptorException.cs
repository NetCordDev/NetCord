namespace NetCord.Gateway.Voice;

public class DaveEncryptorException(string? message) : Exception(message)
{
    internal DaveEncryptorException(Dave.EncryptorResultCode result) : this($"Dave encryptor error: {result}")
    {
    }
}
