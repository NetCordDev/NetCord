using System;

namespace NetCord.Exceptions;

public class InvalidReturnTypeException : ArgumentException
{
    public InvalidReturnTypeException(string message) : base(message)
    {
    }
}
