namespace WildlifeTracker.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task AddAsync(TEntity entity);
        IQueryable<TEntity> All();
        IQueryable<TEntity> AllAsNoTracking();
        void Delete(TEntity entity);
        void Dispose();
        Task<TEntity?> GetByIdAsync(object id);
        Task<int> SaveChangesAsync();
        void Update(TEntity entity);
    }
}