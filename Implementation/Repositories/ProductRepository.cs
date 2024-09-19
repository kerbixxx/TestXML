using TestXML.Context;
using TestXML.Interfaces.Repositories;
using TestXML.Models;

namespace TestXML.Implementation.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly MarketDbContext _marketDbContext;

        public ProductRepository(MarketDbContext marketDbContext) : base(marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public Product FindByName(string name)
        {
            return _marketDbContext.Products.FirstOrDefault(p => p.Name == name);
        }

    }
}
