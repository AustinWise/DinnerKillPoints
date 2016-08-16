using DkpWeb.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        readonly string mGmailUsername;
        readonly string mGmailPassword;

        public AuthMessageSender(IOptions<EmailOptions> options)
        {
            mGmailUsername = options.Value.Username;
            mGmailPassword = options.Value.Password;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            return SendEmailAsync(new MailboxAddress("", email), subject, new TextPart(TextFormat.Plain) { Text = message });
        }

        public Task SendHtmlEmailAsync(string toName, string toEmail, string subject, string message)
        {
            return SendEmailAsync(new MailboxAddress(toName, toEmail), subject, new TextPart(TextFormat.Html) { Text = message });
        }

        async Task SendEmailAsync(MailboxAddress email, string subject, TextPart body)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Austin Wise", mGmailUsername));
            emailMessage.To.Add(email);
            emailMessage.Subject = subject;
            emailMessage.Body = body;

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls).ConfigureAwait(false);
                await client.AuthenticateAsync(mGmailUsername, mGmailPassword);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
