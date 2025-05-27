using System.Linq.Expressions;
using System.Reflection;

using AutoMapper;
using AutoMapper.QueryableExtensions;

using Microsoft.EntityFrameworkCore;

using WildlifeTracker.Constants;
using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;
using WildlifeTracker.Exceptions;

namespace WildlifeTracker.Services.Data
{
    public class GenericService<TEntity>(IDeletableEntityRepository<TEntity> repository, IMapper mapper, IResourceAccessService resourceAccess, ICurrentUserService user) : IGenericService<TEntity>
        where TEntity : BaseEntity
    {
        public async Task<IEnumerable<object>> GetFilteredAndPagedAsync<TDto>(
            int pageNumber,
            int pageSize,
            IEnumerable<string>? filters,
            IEnumerable<string>? fields,
            IEnumerable<string>? orderBy
            )
        {
            try
            {
                ValidatePageParameters(pageNumber, pageSize);

                filters ??= Enumerable.Empty<string>();

                IQueryable<TEntity> query = repository.AllAsNoTracking().Where(e => e.CreatedBy == user.UserId);

                foreach (var filter in filters)
                {
                    var parts = filter.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    if (parts.Length != 3)
                    {
                        throw new ArgumentException($"Invalid filter format: {filter}. Expected format: 'property:operator:value'");
                    }

                    var propertyName = parts[0];
                    var op = parts[1];
                    var value = parts[2];
                    bool hasNot = false;

                    const string notOperator = "n";

                    if (op.StartsWith(notOperator))
                    {
                        op = op[notOperator.Length..];
                        hasNot = true;
                    }

                    var property = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        ?? throw new ArgumentException($"The property '{propertyName}' is not defined for the entity '{typeof(TEntity).Name}'", propertyName);

                    var parameter = Expression.Parameter(typeof(TEntity), "x");
                    var propertyAccess = Expression.Property(parameter, property);

                    object convertedValue;
                    try
                    {
                        convertedValue = Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
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

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(comparison, parameter);
                    query = query.Where(lambda);
                }

                if (orderBy != null && orderBy.Any())
                {
                    foreach (var order in orderBy)
                    {
                        var parts = order.Split(":", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                        if (parts.Length != 2)
                        {
                            throw new ArgumentException($"Invalid order format: {order}. Expected format: 'property:asc|desc'");
                        }
                        var propertyName = parts[0];
                        var direction = parts[1].ToLower();
                        var property = typeof(TEntity).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                            ?? throw new ArgumentException($"The property '{propertyName}' is not defined for the entity '{typeof(TEntity).Name}'", propertyName);
                        var parameter = Expression.Parameter(typeof(TEntity), "x");
                        var propertyAccess = Expression.Property(parameter, property);
                        var lambda = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(propertyAccess, typeof(object)), parameter);
                        query = direction switch
                        {
                            "asc" => query.OrderBy(lambda),
                            "desc" => query.OrderByDescending(lambda),
                            _ => throw new ArgumentException($"Invalid sort direction: {direction}. Expected 'asc' or 'desc'", nameof(order))
                        };
                    }
                }

                var data = await query.Skip(pageNumber * pageSize).Take(pageSize).ProjectTo<TDto>(mapper.ConfigurationProvider).ToListAsync();
                
                if (fields != null && fields.Any())
                {
                    return data.Select(x => fields.ToDictionary(
                        propName => propName,
                        propName => x.GetType().GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)?.GetValue(x)
                            ?? throw new ArgumentException($"The property '{propName}' is not defined for the entity '{typeof(TEntity).Name}'", propName)
                    )).ToList();
                }

                return data.Select(item => (object)item!);
            }
            catch (ArgumentException ex)
            {
                throw new ServiceException(ErrorCodes.GetParametersInvalid, ex.Message);
            }
        }

        public async Task<TDto?> GetByIdAsync<TDto>(int id)
        {
            var entity = await repository.GetByIdAsync(id);

            resourceAccess.Authorize(entity);

            return mapper.Map<TDto>(entity);
        }

        public async Task<TReadDto> AddAsync<TReadDto, TDto>(TDto item)
        {
            if (item == null)
                throw new ServiceException(ErrorCodes.ArgumentNullOrEmpty, "The item cannot be null");

            var entity = mapper.Map<TEntity>(item);

            entity.CreatedBy = user.UserId;

            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();

            return mapper.Map<TReadDto>(entity);
        }

        // TODO: Create the item if it is not found
        public async Task UpdateAsync<TDto>(int id, TDto item)
        {
            if (item == null)
                throw new ServiceException(ErrorCodes.ArgumentNullOrEmpty, "The item cannot be null");

            // Check DTO for Id property (if any) and verify it matches the id in the URL
            PropertyInfo? idProp = item.GetType().GetProperty("Id", BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (idProp != null && idProp.GetValue(item) != (object)id)
                throw new ServiceException(ErrorCodes.IdMismatch, "The 'id' in the URL does not match the 'Id' of the entity");

            var existingItem = await repository.GetByIdAsync(id);

            resourceAccess.Authorize(existingItem);

            if (existingItem == null)
                throw new NotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found");

            mapper.Map(item, existingItem);
            await repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            TEntity? entity = await repository.GetByIdAsync(id);

            if (entity == null)
            {
                throw new NotFoundException($"Entity {typeof(TEntity).Name} with id {id} not found");
            }

            resourceAccess.Authorize(entity);

            repository.Delete(entity);
            await repository.SaveChangesAsync();
        }

        private static void ValidatePageParameters(int pageNumber, int pageSize)
        {
            if (pageNumber < 0)
            {
                throw new ServiceException(ErrorCodes.InvalidPageNumber, "The page number must be greater or equal to 0");
            }

            if (pageSize < 0 || 1000 < pageSize)
            {
                throw new ServiceException(ErrorCodes.InvalidPageSize, "The page size must be between 0 and 1000");
            }
        }

    }
}
