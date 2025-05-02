

namespace WildlifeTracker.Data.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        Task<IEnumerable<T>> GetAllAsNoTrackingAsync(int pageNumber, int pageSize);
        Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Asynchronously searches for entities of type <typeparamref name="T"/> based on dynamic filters provided as a dictionary.
        /// Each filter key can include an optional operator suffix (e.g., "__gt", "__icontains") to apply comparison logic.
        /// Supported operators: eq (default), gt, lt, contains, icontains.
        ///
        /// When comparing strings using operators like "gt" (greater than), "lt" (less than), or "contains", 
        /// the comparison is done lexicographically based on Unicode values of the characters in the string.
        /// This is equivalent to comparing strings alphabetically (e.g., "apple" > "banana" would be false).
        /// String comparisons are performed using the <see cref="string.CompareTo"/> method, which returns a positive, negative, or zero 
        /// value depending on the lexicographical order of the strings.
        /// </summary>
        /// <param name="filters">
        /// A dictionary of property names and values to filter by. Keys can include an operator (e.g., "age__gt").
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a collection of matching entities.
        /// </returns>
        Task<IEnumerable<object>> SearchAsync(int pageNumber, int pageSize, IEnumerable<string>? filters, string[]? fieldsArr);
        Task UpdateAsync(T entity);
    }
}
