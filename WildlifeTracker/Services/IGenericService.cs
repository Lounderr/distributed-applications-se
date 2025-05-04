
using WildlifeTracker.Data.Models.Interfaces;

namespace WildlifeTracker.Services
{
    public interface IGenericService<T>
        where T : class, IIdentifiable
    {
        Task<object> AddAsync<TDto>(TDto item);
        Task DeleteAsync(int id);
        Task<TDto?> GetByIdAsync<TDto>(int id);
        Task<IEnumerable<object>> GetFilteredAndPagedAsync<TDto>(int pageNumber, int pageSize, IEnumerable<string>? filters, IEnumerable<string>? fields, IEnumerable<string>? orderBy);
        Task UpdateAsync<TDto>(int id, TDto item);
    }
}