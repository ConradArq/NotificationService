using System.ComponentModel.DataAnnotations.Schema;

namespace NotificationService.Domain.Models.Entities.External
{
    [Table("AspNetRoles", Schema = "Identity")]
    public class AspNetRole
    {
        public AspNetRole()
        {
            AspNetUserRoles = new HashSet<AspNetUserRole>();
            Id = string.Empty;
            Name = string.Empty;
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public int StatusId { get; set; }
        public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; }
    }
}
