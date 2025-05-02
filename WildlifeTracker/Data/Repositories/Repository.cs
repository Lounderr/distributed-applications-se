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

        public async Task<IEnumerable<object>> SearchAsync(int pageNumber, int pageSize, IEnumerable<string>? filters, string[]? fields)
        {
            this.ValidatePageParameters(pageNumber, pageSize);

            if (filters == null)
                filters = [];

            IQueryable<T> query = context.Set<T>();

            foreach (var filter in filters)
            {
                var parts = filter.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length != 3)
                {
                    throw new ArgumentException($"Invalid filter format: {filter}. Expected format: 'property:operator:value'");
                }

                var propertyName = parts[0];
                var op = parts[1];
                bool hasNot = false;

                const string notOperator = "n";

                if (op.StartsWith(notOperator))
                {
                    op = op.Substring(notOperator.Length);
                    hasNot = true;
                }

                var value = parts[2];

                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                    ?? throw new ArgumentException($"The property '{propertyName}' is not defined for the entity '{typeof(T).Name}'", propertyName);
                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = Expression.Property(parameter, property);

                object convertedValue;

                try
                {
                    convertedValue = Convert.ChangeType(
                        value,
                        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType
                    );
                }
                catch
                {
                    throw new ArgumentException($"Type conversion failed for value '{value}' of property '{propertyName}'");
                }

                Expression comparison;
                try
                {
                    comparison = op.ToLower() switch
                    {
                        "eq" => Expression.Equal(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "gt" => Expression.GreaterThan(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "ge" => Expression.GreaterThanOrEqual(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "lt" => Expression.LessThan(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "le" => Expression.LessThanOrEqual(propertyAccess, Expression.Constant(convertedValue, property.PropertyType)),
                        "contains" when property.PropertyType == typeof(string) =>
                            Expression.Call(propertyAccess, nameof(string.Contains), null, Expression.Constant(value)),
                        "icontains" when property.PropertyType == typeof(string) =>
                            Expression.Call(
                                Expression.Call(propertyAccess, nameof(string.ToLower), null),
                                nameof(string.Contains), null,
                                Expression.Constant(value.ToLower())
                            ),
                        _ => throw new ArgumentException($"Unsupported operator '{op}' for property '{propertyName}'", propertyName)
                    };
                }
                catch (InvalidOperationException ex)
                {
                    throw new ArgumentException($"The operator is not defined for the property '{propertyName}'", propertyName, ex);
                }

                if (hasNot)
                {
                    comparison = Expression.Not(comparison);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            var data = await query.Skip(pageNumber * pageSize).Take(pageSize).ToListAsync();

            if (fields != null)
            {
                return data.Select(x => fields.ToDictionary(
                    propName => propName,
                    propName => x.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.GetValue(x)
                    ?? throw new ArgumentException($"The property '{propName}' is not defined for the entity '{typeof(T).Name}'", propName)
                )).ToList();
            }
            return data;
        }

        private void ValidatePageParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
            {
                throw new BusinessException(ErrorCodes.InvalidPageNumber, "The page number must be greater or equal to 0");
            }

            if (pageSize < 0 || 1000 < pageSize)
            {
                throw new BusinessException(ErrorCodes.InvalidPageSize, "The page size must be between 0 and 1000");
            }
        }
    }
}
