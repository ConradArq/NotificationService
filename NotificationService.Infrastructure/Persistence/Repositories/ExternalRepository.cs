using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models.Entities.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class ExternalRepository : IExternalRepository
    {
        protected readonly ExternalDbContext _context;

        public ExternalRepository(ExternalDbContext context)
        {
            _context = context;
        }

        public async Task<AspNetUser?> GetAspNetUserByAsync(string? userId, string? email)
        {
            var result = await _context.Set<AspNetUser>().FirstOrDefaultAsync(x => !(userId == null && email == null) && (userId == null || x.Id == userId) && (email == null || x.Email == email));
            return result;
        }

        public async Task<List<AspNetUser>> GetAspNetUsersByAsync(string? roleId = null, string? roleName = null)
        {
            if (string.IsNullOrEmpty(roleId) && string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Either roleId or roleName must be provided.");
            }

            IQueryable<AspNetUser> query = _context.Set<AspNetUser>()
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(r => r.Role);

            if (!string.IsNullOrEmpty(roleId))
            {
                query = query.Where(x => x.AspNetUserRoles.Any(r => r.RoleId == roleId));
            }

            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(x => x.AspNetUserRoles.Any(r => r.Role.Name == roleName));
            }

            var result = await query.ToListAsync();

            return await query.ToListAsync();
        }       
    }
}
