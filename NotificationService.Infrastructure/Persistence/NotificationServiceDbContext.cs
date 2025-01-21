using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NotificationService.Shared.Extensions;
using NotificationService.Infrastructure.Persistence.Configurations;
using NotificationService.Infrastructure.Logging.Models.Enums;
using NotificationService.Infrastructure.Logging.Models;
using Microsoft.Extensions.Logging;
using NotificationService.Domain.Models.Entities;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Interfaces.Logging;

namespace NotificationService.Infrastructure.Persistence
{
    public class NotificationServiceDbContext : DbContext
    {
        public string? CurrentUserId { get; set; }
        private readonly IApiLogger _apiLogger;

        public NotificationServiceDbContext(DbContextOptions<NotificationServiceDbContext> options, IApiLogger apiLogger) : base(options)
        {
            _apiLogger = apiLogger;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseDomainModel>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now.InTimeZone();
                        entry.Entity.CreatedBy = CurrentUserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now.InTimeZone();
                        entry.Entity.LastModifiedBy = CurrentUserId;
                        break;
                }
            }

            var changedEntities = ChangeTracker.Entries<BaseDomainModel>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted)
                .Select(entry => new
                {
                    Entry = entry,
                    OriginalState = entry.State,
                    EntityName = Model.FindEntityType(entry.Entity.GetType())?.GetTableName(),
                    Schema = Model.FindEntityType(entry.Entity.GetType())?.GetSchema(),
                    OldValues = JsonSerializer.Serialize(entry.Properties.Select(property => new Dictionary<string, object?>()
                    {
                        { property.Metadata.Name, property.OriginalValue }
                    }))
                })
                .ToList();

            int result = await base.SaveChangesAsync(cancellationToken);

            foreach (var changedEntity in changedEntities)
            {
                var auditLog = new AuditLog
                {
                    EntityName = $"{(changedEntity.Schema != null ? string.Concat(changedEntity.Schema, ".") : string.Empty)}{changedEntity.EntityName}"
                };

                switch (changedEntity.OriginalState)
                {
                    case EntityState.Added:
                        auditLog.EventType = EventType.Create;
                        auditLog.NewData = JsonSerializer.Serialize(changedEntity.Entry.Properties.Select(property => new Dictionary<string, object?>()
                        {
                            { property.Metadata.Name, property.CurrentValue }
                        }));
                        break;

                    case EntityState.Modified:
                        auditLog.EventType = EventType.Update;
                        auditLog.OldData = changedEntity.OldValues;
                        auditLog.NewData = JsonSerializer.Serialize(changedEntity.Entry.Properties.Select(property => new Dictionary<string, object?>()
                        {
                            { property.Metadata.Name, property.CurrentValue }
                        }));
                        break;

                    case EntityState.Deleted:
                        auditLog.EventType = EventType.Delete;
                        auditLog.OldData = changedEntity.OldValues;
                        break;
                }

                _apiLogger.LogInfo(auditLog);
            }

            return result;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("NotificationService");

            builder.ApplyConfiguration(new SmtpConfigConfiguration());
            builder.ApplyConfiguration(new EmailTemplateConfiguration());
            builder.ApplyConfiguration(new EmailNotificationConfiguration());
            builder.ApplyConfiguration(new PushNotificationConfiguration());
        }

        public DbSet<SmtpConfig> SmtpConfig { get; set; }
        public DbSet<EmailTemplate> EmailTemplate { get; set; }
        public DbSet<EmailNotification> EmailNotification { get; set; }
        public DbSet<PushNotification> PushNotification { get; set; }
    }
}
