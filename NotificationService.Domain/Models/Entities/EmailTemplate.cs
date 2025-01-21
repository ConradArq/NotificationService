using NotificationService.Shared.Attributes;

namespace NotificationService.Domain.Models.Entities
{
    [Discoverable]
    public class EmailTemplate : BaseDomainModel
    {
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}

