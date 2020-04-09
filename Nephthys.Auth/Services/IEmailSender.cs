using System.Threading.Tasks;

namespace Nephthys.Auth.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
