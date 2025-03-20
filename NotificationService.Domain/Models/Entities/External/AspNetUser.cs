using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Domain.Models.Entities.External
{
    [Table("AspNetUsers", Schema = "Identity")]
    public class AspNetUser
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new HashSet<AspNetUserRole>();
    }
}
