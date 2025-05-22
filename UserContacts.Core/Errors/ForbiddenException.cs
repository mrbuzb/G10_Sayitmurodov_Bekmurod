using System.Runtime.Serialization;

namespace UserContacts.Core.Errors;

public class ForbiddenException : BaseException
{
    public ForbiddenException() { }
    public ForbiddenException(String message) : base(message) { }
    public ForbiddenException(String message, Exception inner) : base(message, inner) { }
    protected ForbiddenException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
