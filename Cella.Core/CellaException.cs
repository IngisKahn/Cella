namespace Cella.Core;

public class CellaException : Exception
{
    public CellaException() { }

    public CellaException(string message) : base(message) { }

    public CellaException(string message, Exception inner) : base(message, inner) { }
}