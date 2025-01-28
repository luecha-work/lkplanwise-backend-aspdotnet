using Entities;
using IRepository;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository.EntityFramework
{
    public class GenericRepository<T> : IGenericRepositoryEntityFramework<T>
        where T : class
    {
        private readonly LKPlanWiseDbContext _context;

        public GenericRepository(LKPlanWiseDbContext context)
        {
            _context = context;
        }

        public List<T> FindAll() => _context.Set<T>().ToList();
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => _context.Set<T>().Where(expression);
        public T FindOneById(Guid id) => _context.Set<T>().Find(id);
        public void Create(T entity) => _context.Set<T>().Add(entity);
        public async Task CreateAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public void Delete(T entity) => _context.Set<T>().Remove(entity);
        public void DeleteList(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
        }
        public async Task<bool> ExistsAsync(Guid id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            return entity != null;
        }
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }
        public void UpdateList(List<T> entities) => _context.Set<T>().UpdateRange(entities);
    }
}
