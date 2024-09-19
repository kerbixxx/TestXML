using Microsoft.EntityFrameworkCore;
using TestXML.Context;
using TestXML.Interfaces.Repositories;

namespace TestXML.Implementation.Repositories
{
    public class Repository<T> : IRepository<T>  where T : class
    {
        private readonly MarketDbContext _marketDbContext;
        private DbSet<T> _dbSet;
        
        public Repository(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
            _dbSet = _marketDbContext.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public T FindById(int id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        public void Save()
        {
            _marketDbContext.SaveChanges();
        }
    }
}
