using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Models.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            //builder.HasData(
            //    new EmailTemplate
            //    {                     
            //    }
            //);
        }
    }
}
