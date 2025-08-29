using Microsoft.Extensions.Options;
using System.Text;
using Transactions.Application.Settings;

namespace Transactions.Infrastructure.Email
{
    public class EmailTemplateService
    {
        private readonly CompanySettings _companySettings;

        public EmailTemplateService(IOptions<CompanySettings> companySettings)
        {
            _companySettings = companySettings.Value;
        }

        public string GetWelcomeEmailHtml(string username, string password)
        {
            var path = Path.Combine("..", "Transactions.Infrastructure", "Source", "EmailTemplate.html");
            var html = File.ReadAllText(path, Encoding.UTF8);

            html = html.Replace("{{username}}", username)
                       .Replace("{{password}}", password)
                       .Replace("{{companyName}}", _companySettings.Name)
                       .Replace("{{supportEmail}}", _companySettings.SupportEmail)
                       .Replace("{{platformUrl}}", _companySettings.PlatformUrl)
                       .Replace("{{year}}", DateTime.UtcNow.Year.ToString());

            return html;
        }
    }
}
