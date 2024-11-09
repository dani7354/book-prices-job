using System.Net;
using BookPricesJob.Common.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookPricesJob.API.Filter;

public class CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger) : IActionFilter, IOrderedFilter
{
    private readonly ILogger<CustomExceptionFilterAttribute> _logger = logger;
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null)
            return;

        _logger.LogError(context.Exception, context.Exception.Message);

        ProblemDetails? problemDetails;
        if (context.Exception is JobNotFoundException or JobRunNotFoundException)
        {
            problemDetails = new ()
            {
                Title = "Resource not found",
                Detail = context.Exception.Message,
                Status = StatusCodes.Status404NotFound
            };
        }
        else if (context.Exception is UpdateFailedException)
        {
            problemDetails = new ()
            {
                Title = "Update failed",
                Detail = context.Exception.Message,
                Status = StatusCodes.Status400BadRequest
            };
        }
        else if (context.Exception is JobNotCreatedException)
        {
            problemDetails = new ()
            {
                Title = "Not created",
                Detail = context.Exception.Message,
                Status = StatusCodes.Status400BadRequest
            };
        }
        else
        {
            problemDetails = new ()
            {
                Title = "Internal server error",
                Detail = "Something went wrong while processing your request.",
                Status = StatusCodes.Status500InternalServerError
            };
        }

        context.Result = new ObjectResult(problemDetails);
        context.ExceptionHandled = true;
    }
}
