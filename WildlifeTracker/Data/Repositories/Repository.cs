using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

using WildlifeTracker.Constants;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Data.Repositories
{
    public class Repository<T>(ApplicationDbContext context) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet = context.Set<T>();

        public async Task<IEnumerable<T>> GetAllAsNoTrackingAsync(int pageNumber, int pageSize)
        {
            this.ValidatePageParameters(pageNumber, pageSize);

            return await this._dbSet.AsNoTracking().Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize)
        {
            this.ValidatePageParameters(pageNumber, pageSize);

            return await this._dbSet.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await this._dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await this._dbSet.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this._dbSet.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this._dbSet.FindAsync(id);
            if (entity != null)
            {
                this._dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> SearchAsync(int pageNumber, int pageSize, Dictionary<string, string> filters)
        {
            this.ValidatePageParameters(pageNumber, pageSize);

            IQueryable<T> query = context.Set<T>();

            foreach (var filter in filters)
            {
                var (propertyName, op) = ParsePropertyAndOperator(filter.Key);
                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    ?? throw new ArgumentException($"The property {propertyName} is not defined for the entity {typeof(T).Name}", propertyName);
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);

                object convertedValue;
                try
                {
                    convertedValue = Convert.ChangeType(
                        filter.Value,
                        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType
                    );
                }
                catch
                {
                    throw new ArgumentException("Type conversion failed", filter.Value);
                }

                Expression comparison;
                try
                {
                    comparison = op switch
                    {
                        "eq" => Expression.Equal(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "gt" => Expression.GreaterThan(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "lt" => Expression.LessThan(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "contains" when property.PropertyType == typeof(string) =>
                            Expression.Call(propertyAccess, nameof(string.Contains), null, Expression.Constant(filter.Value)),
                        "icontains" when property.PropertyType == typeof(string) =>
                            Expression.Call(
                                Expression.Call(propertyAccess, nameof(string.ToLower), null),
                                nameof(string.Contains), null,
                                Expression.Constant(filter.Value.ToLower())
                            ),
                        _ => throw new ArgumentException($"Unsupported operator '{op}' for property '{propertyName}'", propertyName)
                    };
                }
                catch (InvalidOperationException ex)
                {
                    throw new ArgumentException($"The operator is not defined for the property '{propertyName}'", propertyName, ex);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            return await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();
        }

        private static (string property, string op) ParsePropertyAndOperator(string key)
        {
            var parts = key.Split("__", 2); // e.g., name__icontains

            if (parts.Length == 1)
                return (parts[0], "eq");
            else if (parts.Length == 2)
                return (parts[0], parts[1]);
            else
                throw new ValidationException($"Invalid number of arguments ({parts}) for filter '{key}'");
        }

        private void ValidatePageParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
            {
                throw new CustomValidationException(ErrorCodes.InvalidPageNumber, "The page number must be greater or equal to 0");
            }

            if (pageSize < 0 || 1000 < pageSize)
            {
                throw new CustomValidationException(ErrorCodes.InvalidPageSize, "The page size must be between 0 and 1000");
            }
        }
    }
}
