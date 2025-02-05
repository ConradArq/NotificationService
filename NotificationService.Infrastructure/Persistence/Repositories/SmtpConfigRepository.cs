using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class SmtpConfigRepository : GenericRepository<SmtpConfig>, ISmtpConfigRepository
    {
        public SmtpConfigRepository(NotificationServiceDbContext context) : base(context)
        {
        }
    }
}
