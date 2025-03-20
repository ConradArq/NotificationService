using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Domain.Models.Entities.External
{
    [Table("AspNetUserRoles", Schema = "Identity")]
    public partial class AspNetUserRole
    {
        public string UserId { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public int StatusId { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;
        public virtual AspNetUser User { get; set; } = null!;
    }
}
