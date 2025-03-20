using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Models.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class SmtpConfigConfiguration : IEntityTypeConfiguration<SmtpConfig>
    {
        public void Configure(EntityTypeBuilder<SmtpConfig> builder)
        {
            //builder.HasData(
            //    new SmtpConfig
            //    {                     
            //    }
            //);
        }
    }
}
