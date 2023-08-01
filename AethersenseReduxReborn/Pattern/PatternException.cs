using System;

namespace AethersenseReduxReborn.Pattern;

public class PatternException : Exception
{
    public PatternException() { }

    public PatternException(string message)
        : base(message) { }

    public PatternException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class PatternExpiredException : PatternException
{
    public PatternExpiredException() { }

    public PatternExpiredException(string message)
        : base(message) { }

    public PatternExpiredException(string message, Exception innerException)
        : base(message, innerException) { }

}