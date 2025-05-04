using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyDub.Content.Middlewares;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is null)
            return;

        context.Result = new ObjectResult(context.Exception.Message)
        {
            StatusCode = (int)HttpStatusCode.BadRequest,
        };

        context.ExceptionHandled = true;
    }
}
