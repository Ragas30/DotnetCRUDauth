namespace DotnetCRUD.Exceptions;

public class ConflictException : ApiException
{
    public ConflictException(string code, string message)
        : base(StatusCodes.Status409Conflict, code, message)
    {
    }
}
