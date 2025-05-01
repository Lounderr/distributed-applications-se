using System.Linq.Expressions;
using System.Reflection;

using Microsoft.EntityFrameworkCore;

namespace WildlifeTracker.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            this._context = context;
            this._dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsNoTrackingAsync()
        {
            return await this._dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this._dbSet.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await this._dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await this._dbSet.AddAsync(entity);
            await this._context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            this._dbSet.Update(entity);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await this._dbSet.FindAsync(id);
            if (entity != null)
            {
                this._dbSet.Remove(entity);
                await this._context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<T>> SearchAsync(Dictionary<string, string> filters)
        {
            IQueryable<T> query = this._context.Set<T>();

            foreach (var filter in filters)
            {
                var (propertyName, op) = this.ParsePropertyAndOperator(filter.Key);
                var property = typeof(T).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    throw new ArgumentException($"The property {propertyName} is not defined for the entity {typeof(T).Name}");

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
                    throw new ArgumentException("Type conversion failed");
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
                        _ => throw new ArgumentException($"Unsupported operator '{op}' for property '{propertyName}'")
                    };
                }
                catch (InvalidOperationException ex)
                {
                    throw new ArgumentException($"The operator is not defined for the property '{propertyName}'", ex);
                }

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                query = query.Where(lambda);
            }

            return await query.ToListAsync();
        }

        private (string property, string op) ParsePropertyAndOperator(string key)
        {
            var parts = key.Split("__", 2); // e.g., name__icontains

            if (parts.Length == 1)
                return (parts[0], "eq");
            else if (parts.Length == 2)
                return (parts[0], parts[1]);
            else
                throw new ArgumentException($"Invalid number of arguments ({parts}) for filter '{key}'");
        }
    }
}
