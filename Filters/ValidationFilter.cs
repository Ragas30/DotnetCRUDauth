using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DotnetCRUD.Filters;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var argumentType = argument.GetType();
            var validatorInterface = typeof(IValidator<>).MakeGenericType(argumentType);
            var validator = context.HttpContext.RequestServices.GetService(validatorInterface);
            if (validator is null)
            {
                continue;
            }

            var validateAsyncMethod = validatorInterface.GetMethod(
                nameof(IValidator<object>.ValidateAsync),
                new[] { argumentType, typeof(CancellationToken) });

            if (validateAsyncMethod is null)
            {
                continue;
            }

            var task = (Task<ValidationResult>)validateAsyncMethod.Invoke(
                validator,
                new object[] { argument, CancellationToken.None })!;
            var validationResult = await task;

            if (!validationResult.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    errors = validationResult.Errors.Select(error => new
                    {
                        field = error.PropertyName,
                        message = error.ErrorMessage
                    })
                });
                return;
            }
        }

        await next();
    }
}
