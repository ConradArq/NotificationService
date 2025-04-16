using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Interfaces.Strategies
{
    /// <summary>
    /// Defines a strategy for deleting an entity, which may be implemented as a hard or soft delete.
    /// </summary>
    public interface IDeletionStrategy<T> where T : BaseDomainModel
    {
        void Delete(T entity, IUnitOfWork unitOfWork);
    }
}
