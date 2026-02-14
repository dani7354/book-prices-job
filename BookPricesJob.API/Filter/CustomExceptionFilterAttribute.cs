using BookPricesJob.Application.Exception;
using BookPricesJob.Common.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace BookPricesJob.API.Filter;

public class CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger) : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        logger.LogError(context.Exception, context.Exception.Message);

        var statusCode = context.Exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            DbUpdateConcurrencyException => StatusCodes.Status412PreconditionFailed,
            JobRunUpdateInvalidRequestException => StatusCodes.Status400BadRequest,

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
