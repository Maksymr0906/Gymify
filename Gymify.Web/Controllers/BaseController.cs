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
        var errors = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);

        string stringErrors = string.Join("<br>", errors);

        if (!string.IsNullOrEmpty(stringErrors))
        {
            NotifyError(stringErrors);
        }
    }
}
