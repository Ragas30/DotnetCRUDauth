namespace DotnetCRUD.Exceptions;

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string code, string message)
        : base(StatusCodes.Status401Unauthorized, code, message)
    {
    }
}
