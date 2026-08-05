namespace DotnetCRUD.Exceptions;

public class BusinessRuleException : ApiException
{
    public BusinessRuleException(string code, string message)
        : base(StatusCodes.Status400BadRequest, code, message)
    {
    }
}
