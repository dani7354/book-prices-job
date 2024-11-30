using BookPricesJob.Common.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookPricesJob.API.Filter;

public class CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger) : IExceptionFilter
{
    private readonly ILogger<CustomExceptionFilterAttribute> _logger = logger;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, context.Exception.Message);

        var statusCode = context.Exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            UpdateFailedException => StatusCodes.Status400BadRequest,
            DatabaseException => StatusCodes.Status500InternalServerError,

            _ => StatusCodes.Status500InternalServerError
        };

        context.Result = new ObjectResult(new ProblemDetails
        {
            Title = context.Exception.Message,
            Status = statusCode
        })
        {
            StatusCode = statusCode
        };
    }
}
