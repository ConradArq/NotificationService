using NotificationService.Shared.Attributes;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Domain.Models.Entities
{
    [Discoverable]
    public class EmailNotification : Notification
    {
        private List<string>? _to;
        private List<string>? _cc;
        private List<string>? _bcc;

        public int? TemplateId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        [NotMapped]
        public List<string> To
        {
            get
            {
                if (_to == null)
                {
                    _to = ToRecipients == null || ToRecipients == string.Empty
                        ? new List<string>()
                        : ToRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return _to;
            }
            set
            {
                _to = value ?? new List<string>();
                ToRecipients = _to.Any() ? string.Join(",", _to) : null;
            }
        }

        [NotMapped]
        public List<string> CC
        {
            get
            {
                if (_cc == null)
                {
                    _cc = CcRecipients == null || CcRecipients == string.Empty
                        ? new List<string>()
                        : CcRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return _cc;
            }
            set
            {
                _cc = value ?? new List<string>();
                CcRecipients = _cc.Any() ? string.Join(",", _cc) : null;
            }
        }

        [NotMapped]
        public List<string> BCC
        {
            get
            {
                if (_bcc == null)
                {
                    _bcc = BccRecipients == null || BccRecipients == string.Empty
                        ? new List<string>()
                        : BccRecipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                return _bcc;
            }
            set
            {
                _bcc = value ?? new List<string>();
                BccRecipients = _bcc.Any() ? string.Join(",", _bcc) : null;
            }
        }

        // Mapped properties to store comma-separated values in the database
        public string? ToRecipients { get; set; }
        public string? CcRecipients { get; set; }
        public string? BccRecipients { get; set; }

        [NotMapped]
        public List<FileAttachment>? FileAttachments { get; set; } = new();

        /// <summary>
        /// Adds a recipient email address to the list of "To" recipients. 
        /// This method ensures that the <see cref="ToRecipients"/> property is updated 
        /// to reflect the current list of recipients as a comma-separated string.
        /// Use this method instead of directly modifying <see cref="To"/> to ensure 
        /// synchronization between the <see cref="To"/> list and the <see cref="ToRecipients"/> property.
        /// </summary>
        /// <param name="email">The email address to add to the "To" list.</param>
        public void AddRecipient(string email)
        {
            To.Add(email);
            ToRecipients = string.Join(",", To);
        }

        /// <summary>
        /// Adds a recipient email address to the list of "CC" recipients. 
        /// This method ensures that the <see cref="CcRecipients"/> property is updated 
        /// to reflect the current list of recipients as a comma-separated string.
        /// Use this method instead of directly modifying <see cref="CC"/> to ensure 
        /// synchronization between the <see cref="CC"/> list and the <see cref="CcRecipients"/> property.
        /// </summary>
        /// <param name="email">The email address to add to the "CC" list.</param>
        public void AddCcRecipient(string email)
        {
            CC.Add(email);
            CcRecipients = string.Join(",", CC);
        }

        /// <summary>
        /// Adds a recipient email address to the list of "BCC" recipients. 
        /// This method ensures that the <see cref="BccRecipients"/> property is updated 
        /// to reflect the current list of recipients as a comma-separated string.
        /// Use this method instead of directly modifying <see cref="BCC"/> to ensure 
        /// synchronization between the <see cref="BCC"/> list and the <see cref="BccRecipients"/> property.
        /// </summary>
        /// <param name="email">The email address to add to the "BCC" list.</param>
        public void AddBccRecipient(string email)
        {
            BCC.Add(email);
            BccRecipients = string.Join(",", BCC);
        }
    }
}