using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public class BaseController : Controller
{
    protected bool IsUkrainian => CultureInfo.CurrentCulture.Name == "uk-UA" || CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "uk";
    protected void NotifySuccess(string message)
    {
        TempData["Success"] = message;
    }

    protected void NotifyError(string message)
    {
        TempData["Error"] = message;
    }

    protected void NotifyWarning(string message)
    {
        TempData["Warning"] = message;
    }

    protected void NotifyModelStateErrors()
    {
        // Знаходимо першу помилку в ModelState
        var firstError = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault();

        // Якщо помилка є — показуємо її через Alertify
        if (!string.IsNullOrEmpty(firstError))
        {
            NotifyError(firstError);
        }
    }
}
