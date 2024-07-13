using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace TelegramPoster.Application.Validator;
public static class ValidationExtensions
{
    public static bool AssertFound<T>(this T? value, ModelStateDictionary modelState) where T : class
    {
        if (value == null)
        {
            modelState.AddModelError(typeof(T).Name, "Запрашиваемый ресурс не найден.");
            return false;
        }
        return true;
    }

    public static void AssertFound<T>(this T? value) where T : class
    {
        if (value == null)
        {
            throw new BadRequestException(typeof(T).Name, "Запрашиваемый ресурс не найден.");
        }
    }

    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is BadRequestException badRequestException)
            {
                context.Result = new ObjectResult(new { message = badRequestException.Message })
                {
                    StatusCode = 400
                };
                context.ExceptionHandled = true;
            }
        }
    }
}