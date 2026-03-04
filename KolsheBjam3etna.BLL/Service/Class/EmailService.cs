using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace KolsheBjam3etna.BLL.Service.Class
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string toEmail, string subject, string body)
        {
            var host = _config["EmailSettings:Host"]!;
            var port = int.Parse(_config["EmailSettings:Port"]!);
            var fromEmail = _config["EmailSettings:FromEmail"]!;
            var fromName = _config["EmailSettings:FromName"] ?? "App";
            var appPassword = _config["EmailSettings:AppPassword"]!;

            using var smtp = new SmtpClient(host, port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(fromEmail, appPassword)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await smtp.SendMailAsync(message);
        }
    }
}