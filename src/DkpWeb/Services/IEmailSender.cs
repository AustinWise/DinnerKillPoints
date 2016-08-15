using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task SendHtmlEmailAsync(string toName, string toEmail, string subject, string message);
    }
}
