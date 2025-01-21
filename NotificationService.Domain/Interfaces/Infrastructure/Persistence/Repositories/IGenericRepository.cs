using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces.Infrastructure.Persistence.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T Create(T entity);
        ICollection<T> CreateRange(ICollection<T> entities);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entities);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>>? includesAndThenIncludes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true);
        Task<(IReadOnlyList<T> Data, int TotalItems)> GetPaginatedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>>? includesAndThenIncludes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true);
        Task<T?> GetSingleAsync(int id, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>>? includesAndThenIncludes = null, bool disableTracking = false);
        T Update(T entity);
        ICollection<T> UpdateRange(ICollection<T> entities);
    }
}
