using NotificationService.Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces.Repositories
{
    public interface ISmtpConfigRepository : IGenericRepository<SmtpConfig>
    {
    }
}
