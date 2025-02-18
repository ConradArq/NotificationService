using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T Create(T entity);
        ICollection<T> CreateRange(ICollection<T> entities);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entities);

        /// <summary>
        /// Retrieves a list of entities from the database with optional filtering, ordering, and eager loading.
        /// </summary>
        /// <typeparam name="T">The entity type being queried.</typeparam>
        /// <param name="predicate">
        ///     (Optional) A filter expression to apply to the query. Example: `x => x.IsActive` to retrieve only active records.
        /// </param>
        /// <param name="includesAndThenIncludes">
        ///     (Optional) A dictionary specifying related entities to include.
        ///     - The **key** (`Expression<Func<T, object>>`) is the primary navigation property to include.
        ///     - The **value** (`List<Expression<Func<object, object>>>?`) contains **nested navigation properties** (via `.ThenInclude()`).
        ///     
        ///     **Example Usage:**
        ///     
        ///     new Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>
        ///     {
        ///         { x => x.NavigationProperty1, null }, // Simple Include
        ///         { x => x.NavigationProperty2, new List<Expression<Func<object, object>>>
        ///             { x => x.SubNavigationProperty } } // Include + ThenInclude
        ///     }
        /// </param>
        /// <param name="orderBy">
        ///     (Optional) A function to apply ordering to the query. Example: `query => query.OrderBy(x => x.Name)`.
        /// </param>
        /// <param name="disableTracking">
        ///     (Optional, default: `true`) Whether to disable tracking (`AsNoTracking()`) for read-only queries.
        /// </param>
        /// <returns>
        ///     A **read-only list** of the requested entity type (`IReadOnlyList<T>`) from the database.
        /// </returns>
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true);

        /// <summary>
        /// Retrieves a paginated list of entities from the database.
        /// Works like <see cref="GetAsync"/> but adds pagination.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="pageNumber">The page number (1-based index). Defaults to 1 if less than or equal to 0.</param>
        /// <param name="pageSize">The number of records per page. Must be greater than 0.</param>
        /// <returns>
        ///     A tuple containing:
        ///     - **Data**: The paginated list of entities.
        ///     - **TotalItems**: The total number of records matching the criteria.
        /// </returns>
        Task<(IReadOnlyList<T> Data, int TotalItems)> GetPaginatedAsync(int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true);

        /// <summary>
        /// Retrieves a single entity by its ID.
        /// Works like <see cref="GetAsync"/> but fetches only one entity.
        /// </summary>
        /// <param name="id">The unique identifier of the entity.</param>
        /// <returns>The entity if found; otherwise, `null`.</returns>
        Task<T?> GetSingleAsync(int id, Dictionary<Expression<Func<T, object>>, List<Expression<Func<object, object>>>?>? includesAndThenIncludes = null, bool disableTracking = false);
        T Update(T entity);
        ICollection<T> UpdateRange(ICollection<T> entities);
    }
}
