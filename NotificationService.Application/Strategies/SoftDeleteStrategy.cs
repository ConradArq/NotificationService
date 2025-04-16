using NotificationService.Application.Interfaces.Strategies;
using NotificationService.Domain.Enums;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Strategies
{
    /// <summary>
    /// A deletion strategy that performs a soft delete by setting the status of the entity and its related children to inactive.
    /// </summary>
    /// <typeparam name="T">The type of the entity to soft delete, which must implement <see cref="IBaseDomainModel"/>.</typeparam>
    public class SoftDeleteStrategy<T> : IDeletionStrategy<T> where T : BaseDomainModel
    {
        public static readonly SoftDeleteStrategy<T> Instance = new();

        private SoftDeleteStrategy() { }

        public void Delete(T entity, IUnitOfWork unitOfWork)
        {
            if (entity is BaseDomainModel baseEntity)
            {
                baseEntity.StatusId = (int)Status.Inactive;
            }

            var collections = typeof(T).GetProperties()
                .Where(p =>
                    typeof(IEnumerable<BaseDomainModel>).IsAssignableFrom(p.PropertyType) &&
                    p.PropertyType != typeof(string));

            foreach (var prop in collections)
            {
                if (prop.GetValue(entity) is IEnumerable<BaseDomainModel> children)
                {
                    foreach (var child in children)
                        child.StatusId = (int)Status.Inactive;
                }
            }
        }      
    }
}
