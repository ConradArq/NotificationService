using NotificationService.Application.Interfaces.Strategies;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Strategies
{
    /// <summary>
    /// A deletion strategy that performs a hard delete by removing the entity from the database context.
    /// </summary>
    /// <typeparam name="T">The type of the entity to delete.</typeparam>
    public class HardDeleteStrategy<T> : IDeletionStrategy<T> where T : BaseDomainModel
    {
        public static readonly HardDeleteStrategy<T> Instance = new();

        private HardDeleteStrategy() { }

        public void Delete(T entity, IUnitOfWork unitOfWork)
        {
            unitOfWork.Repository<T>().Delete(entity);
        }
    }
}
