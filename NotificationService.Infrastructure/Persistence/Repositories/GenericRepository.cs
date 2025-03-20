using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Domain.Models;
using System.Linq.Expressions;

namespace NotificationService.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseDomainModel
    {
        protected readonly NotificationServiceDbContext _context;

        public GenericRepository(NotificationServiceDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(
            Expression<Func<T, bool>>? predicate = null, 
            Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null, 
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includesAndThenIncludes != null)
            {
                query = includesAndThenIncludes.Aggregate(query, (current, x) =>
                {
                    var queryWithInclude = current.Include(x.Key);

                    if (x.Value != null)
                    {
                        foreach (var thenInclude in x.Value)
                        {
                            if (thenInclude != null)
                                queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                        }
                    }
                    return queryWithInclude;
                });
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        public virtual async Task<(IReadOnlyList<T> Data, int TotalItems)> GetPaginatedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? predicate = null,
            Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool disableTracking = true)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) throw new Exception("Page size must be greater than 0");

            IQueryable<T> query = _context.Set<T>();

            if (disableTracking)
                query = query.AsNoTracking();

            if (includesAndThenIncludes != null)
            {
                query = includesAndThenIncludes.Aggregate(query, (current, x) =>
                {
                    var queryWithInclude = current.Include(x.Key);

                    if (x.Value != null)
                    {
                        foreach (var thenInclude in x.Value)
                        {
                            if (thenInclude != null)
                                queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                        }
                    }

                    return queryWithInclude;
                });
            }

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            var totalItems = await query.CountAsync();

            var paginatedItems = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (paginatedItems, totalItems);
        }


        public virtual async Task<T?> GetSingleAsync(
            int id, 
            Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null, 
            bool disableTracking = false)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query = query.AsNoTracking();

            if (includesAndThenIncludes != null)
            {
                query = includesAndThenIncludes.Aggregate(query, (current, x) =>
                {
                    var queryWithInclude = current.Include(x.Key);

                    if (x.Value != null)
                    {
                        foreach (var thenInclude in x.Value)
                        {
                            if (thenInclude != null)
                                queryWithInclude = queryWithInclude.ThenInclude(thenInclude);
                        }
                    }
                    return queryWithInclude;
                });
            }

            T? result = await query.FirstOrDefaultAsync(cd => cd.Id == id);
            return result;
        }

        public virtual T Create(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public virtual ICollection<T> CreateRange(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<T>().Add(entity);
            }
            return entities;
        }

        public virtual T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }

        public virtual ICollection<T> UpdateRange(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<T>().Update(entity);
            }
            return entities;
        }

        public virtual void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual void DeleteRange(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _context.Set<T>().Remove(entity);
            }
        }
    }
}
