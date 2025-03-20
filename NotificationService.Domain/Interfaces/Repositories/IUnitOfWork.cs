using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IEmailTemplateRepository EmailTemplateRepository { get; }
        IEmailNotificationRepository EmailNotificationRepository { get; }
        IPushNotificationRepository PushNotificationRepository { get; }
        ISmtpConfigRepository SmtpConfigRepository { get; }
        IExternalRepository ExternalRepository { get; }

        Task CompleteTransactionAsync(Func<Task> functionTransaction);
        void Dispose();
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseDomainModel;
        object GetRepository(Type entityType);
        Task<int> SaveAsync();
    }
}