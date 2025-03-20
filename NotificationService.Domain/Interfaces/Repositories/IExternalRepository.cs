using NotificationService.Domain.Models.Entities.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces.Repositories
{
    public interface IExternalRepository
    {
        Task<AspNetUser?> GetAspNetUserByAsync(string? userId, string? email);
        Task<List<AspNetUser>> GetAspNetUsersByAsync(string? roleId = null, string? roleName = null);
    }
}
