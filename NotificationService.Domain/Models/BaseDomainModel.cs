using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace NotificationService.Domain.Models
{
    public abstract class BaseDomainModel
    {
        private static readonly Dictionary<string, PropertyInfo> PropertyCache = new Dictionary<string, PropertyInfo>();

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }

        public int StatusId { get; set; } = (int)Enums.Status.Active;

        // Validates property based on data annotations and caches properties to speed up reflection
        protected void ValidateProperty(string propertyName)
        {
            if (!PropertyCache.TryGetValue(propertyName, out var property))
            {
                property = GetType().GetProperty(propertyName);

                if (property == null)
                {
                    throw new ValidationException($"Property '{propertyName}' does not exist.");
                }

                PropertyCache[propertyName] = property;
            }

            var validationContext = new ValidationContext(this) { MemberName = propertyName };
            Validator.ValidateProperty(property.GetValue(this), validationContext);
        }
    }
}
