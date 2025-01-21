
namespace NotificationService.Domain.Models
{
    public class FileAttachment
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; } = new byte[0];
    }
}
