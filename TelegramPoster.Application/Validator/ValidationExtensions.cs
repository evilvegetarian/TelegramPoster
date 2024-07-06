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
}