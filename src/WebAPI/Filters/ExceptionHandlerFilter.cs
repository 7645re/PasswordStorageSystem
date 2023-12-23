using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebAPI.Filters;

public class ExceptionHandlerFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        var errorDetail = GetExceptionDetail(exception, context.RouteData);
        var errorsResponse = new ExceptionsResponse
        {
            Exceptions = new[]
            {
                errorDetail
            }
        };

        context.Result = new ObjectResult(errorsResponse)
        {
            StatusCode = (int)HttpStatusCode.BadRequest
        };
    }
    
    private ExceptionDetail GetExceptionDetail(Exception exception, RouteData routeData)
    {
        var parameter = routeData.Values["action"]!.ToString()!;
        var errorId = Guid.NewGuid();

        return new ExceptionDetail
        {
            Error = "InvalidRequest",
            Parameter = parameter,
            Code = errorId,
            Message = exception.Message
        };
    }
}