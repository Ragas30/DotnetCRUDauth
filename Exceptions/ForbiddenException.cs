namespace DotnetCRUD.Exceptions;

public class ForbiddenException : ApiException
{
    public ForbiddenException(string code, string message)
        : base(StatusCodes.Status403Forbidden, code, message)
    {
    }
}
