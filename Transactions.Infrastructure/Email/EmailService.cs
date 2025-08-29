using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Transactions.Application.Email;
using Transactions.Application.Settings;
using Microsoft.Extensions.Options;


namespace Transactions.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailTemplateService _templateService;
        private readonly SmtpSettings _smtpSettings;


        public EmailService(EmailTemplateService templateService, IOptions<SmtpSettings> smtpSettings)
        {
            _templateService = templateService;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendWelcomeEmailAsync(string username, string email, string password)
        {
            var html = _templateService.GetWelcomeEmailHtml(username, password);

            var message = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From, _smtpSettings.DisplayName),
                Subject = $"Bienvenue chez {_smtpSettings.DisplayName}",
                Body = html,
                IsBodyHtml = true
            };

            message.To.Add(email);

            using var smtp = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                EnableSsl = true
            };

            await smtp.SendMailAsync(message);
        }
    }
}
