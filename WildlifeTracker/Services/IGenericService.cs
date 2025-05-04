
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Services
{
    public interface IGenericService<T>
        where T : class, IIdentifiable
    {
        Task AddAsync(T item);
        Task DeleteAsync(int id);
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<object>> GetFilteredAndPagedAsync(int pageNumber, int pageSize, IEnumerable<string>? filters, IEnumerable<string>? fields, IEnumerable<string>? orderBy);
        Task UpdateAsync(int id, T item);
    }
}