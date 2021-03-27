using System.Threading.Tasks;

namespace shoppingApp.WebUI.EmailServices
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}