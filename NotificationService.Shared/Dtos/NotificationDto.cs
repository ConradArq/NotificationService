using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Shared.Dtos
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }

        public int StatusId { get; set; }
        public string? UserId { get; set; }
        public string? RoleId { get; set; }
    }
}
