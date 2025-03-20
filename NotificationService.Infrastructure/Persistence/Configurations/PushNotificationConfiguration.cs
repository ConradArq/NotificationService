using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationService.Domain.Models.Entities;

namespace NotificationService.Infrastructure.Persistence.Configurations
{
    public class PushNotificationConfiguration : IEntityTypeConfiguration<PushNotification>
    {
        public void Configure(EntityTypeBuilder<PushNotification> builder)
        {
            //builder.HasData(
            //    new PushNotification
            //    { }
            //);
        }
    }
}
