namespace NotificationService.Services.Abstract
{
    public interface IEmailSender
    {
        void SendEmail(string to, string subject, string htmlBody, string attachmentFilePath);
    }
}
