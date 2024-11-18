using Microsoft.Extensions.Logging;
using NotificationService.Services.Abstract;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Services.Concrete
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(ILogger<SmtpEmailSender> logger)
        {
            _logger = logger;
        }

        public void SendEmail(string to, string subject, string htmlBody, string attachmentFilePath)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.yourserver.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("yourUsername", "yourPassword"), // Replace with your credentials
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("youremail@example.com"),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                if (!string.IsNullOrEmpty(attachmentFilePath) && File.Exists(attachmentFilePath))
                {
                    var attachment = new Attachment(attachmentFilePath);
                    mailMessage.Attachments.Add(attachment);
                }

                smtpClient.Send(mailMessage);
                _logger.LogInformation($"Email sent successfully to {to} with subject '{subject}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email to {to}. Error: {ex.Message}");
            }
        }
    }
}
