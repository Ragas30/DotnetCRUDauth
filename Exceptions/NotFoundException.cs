namespace DotnetCRUD.Exceptions;

public class NotFoundException : ApiException
{
    public NotFoundException(string code, string message)
        : base(StatusCodes.Status404NotFound, code, message)
    {
    }
}
