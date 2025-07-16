using Microsoft.EntityFrameworkCore;
using OrderService.Repository.ApplicationContext;
using OrderService.Repository.Interfaces;

namespace OrderService.Repository.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly OrderContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(OrderContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }
        public async Task<T?> GetByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            _dbSet.Remove(entity);
        }
    }
}
