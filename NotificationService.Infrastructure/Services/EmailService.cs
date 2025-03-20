using System.Net.Mail;
using System.Net;
using NotificationService.Domain.Models.Entities;
using NotificationService.Infrastructure.Interfaces.Services;

namespace NotificationService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(EmailNotification emailNotification, SmtpConfig smtpConfig)
        {
            using var message = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(smtpConfig.FromAddress, smtpConfig.FromName),
                Subject = emailNotification.Subject,
                Body = emailNotification.Body
            };

            foreach (var toEmail in emailNotification.To ?? new List<string>())
            {
                message.To.Add(new MailAddress(toEmail));
            }

            foreach (var ccEmail in emailNotification.CC ?? new List<string>())
            {
                message.CC.Add(new MailAddress(ccEmail));
            }

            foreach (var bccEmail in emailNotification.BCC ?? new List<string>())
            {
                message.Bcc.Add(new MailAddress(bccEmail));
            }

            var attachments = new List<Attachment>();

            try
            {
                if (emailNotification.FileAttachments != null && emailNotification.FileAttachments.Any())
                {
                    foreach (var fileAttachment in emailNotification.FileAttachments)
                    {
                        if (fileAttachment.Content.Length > 0)
                        {
                            var memoryStream = new MemoryStream(fileAttachment.Content);
                            var attachment = new Attachment(memoryStream, fileAttachment.FileName);
                            attachments.Add(attachment);
                            message.Attachments.Add(attachment);
                        }
                    }
                }

                using var smtpClient = new SmtpClient(smtpConfig.Server, smtpConfig.Port)
                {
                    Credentials = new NetworkCredential(smtpConfig.UserName, smtpConfig.Password),
                    EnableSsl = smtpConfig.EnableSsl
                };

                await smtpClient.SendMailAsync(message);
            }
            finally
            {
                foreach (var attachment in attachments)
                {
                    attachment.Dispose();
                }
            }
        }
    }
}
