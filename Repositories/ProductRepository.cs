using TestXML.Context;
using TestXML.Models;

namespace TestXML.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly MarketDbContext _marketDbContext;

        public ProductRepository(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public void Add(Product entity)
        {
            _marketDbContext.Products.Add(entity);
        }

        public Product FindById(int id)
        {
            return _marketDbContext.Products.FirstOrDefault(p => p.Id == id);
        }

        public Product FindByName(string name)
        {
            return _marketDbContext.Products.FirstOrDefault(p => p.Name == name);
        }
    }
}
