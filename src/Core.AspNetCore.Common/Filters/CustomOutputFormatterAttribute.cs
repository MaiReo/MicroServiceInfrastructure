using Core.Web.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Core.Web
{
    [AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CustomOutputFormatterAttribute : Attribute, IAsyncActionFilter, IExceptionFilter, IFilterMetadata
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var result = await next();

            if (result.Canceled)
            {
                return;
            }
            if (result.Exception != default)
            {
                return;
            }
            if (result.Result == default)
            {
                return;
            }
            if (!(result.Result is ObjectResult objectResult))
            {
                return;
            }
            if (objectResult.Value is null)
            {
                return;
            }
            if (objectResult.Value is WrapResultBase wrapResultBase)
            {
                return;
            }
            var @object = new WrapResult<object>(objectResult.Value);
            objectResult.Value = @object;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }
            var statusCode = GetStatusCode(context);

            var @object = new WrapResult(new ErrorInfo
            {
                Message = context.Exception.Message,
            });
            var objectResult = new ObjectResult(@object)
            {
                StatusCode = statusCode
            };
            objectResult.ContentTypes?.Add(MediaTypeHeaderValue.Parse("application/json"));
            context.Result = objectResult;
            context.ExceptionHandled = true;
            context.Exception = null;
        }

        private int? GetStatusCode(ExceptionContext context)
        {
            return 200;
        }
    }
}