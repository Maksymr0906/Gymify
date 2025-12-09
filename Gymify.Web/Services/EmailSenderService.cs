using Microsoft.AspNetCore.Identity.UI.Services;

namespace Gymify.Web.Services
{
    public class EmailSenderService : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Email: {email}, Subject: {subject}, Message: {htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
