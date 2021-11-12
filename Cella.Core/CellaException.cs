using System.Runtime.Serialization;

namespace Cella.Core;

[Serializable]
public class CellaException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public CellaException()
    {
    }

    public CellaException(string message) : base(message)
    {
    }

    public CellaException(string message, Exception inner) : base(message, inner)
    {
    }

    protected CellaException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}