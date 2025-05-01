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
    }
}
