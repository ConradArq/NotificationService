using NotificationService.Shared.Attributes;

namespace NotificationService.Domain.Models.Entities
{
    [Discoverable]
    public class SmtpConfig : BaseDomainModel
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; }
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool EnableSsl { get; set; }
    }
}