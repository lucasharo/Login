using Core.Interfaces;
using Entities.Settings;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public void SendEmail(string email, string subject, string message)
        {
            string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;

            MailMessage mail = new MailMessage()
            {
                From = new MailAddress(_emailSettings.UsernameEmail)
            };

            mail.To.Add(new MailAddress(toEmail));

            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;

            using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
            {
                smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                smtp.EnableSsl = true;
                smtp.SendMailAsync(mail).GetAwaiter();
            }
        }
    }
}