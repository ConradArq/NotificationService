using NotificationService.Infrastructure.Logging.Models.Enums;
using NotificationService.Infrastructure.Logging.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Interfaces.Logging
{
    public interface IApiLogger
    {
        void LogInfo(AuditLog logEntry);

        void LogError(ErrorLog logEntry);
    }
}
