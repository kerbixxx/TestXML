using TestXML.Context;
using TestXML.Models;

namespace TestXML.Repositories
{
    public class ProductOrderRepository : IRepository<ProductOrder>
    {
        private readonly MarketDbContext _marketDbContext;

        public ProductOrderRepository(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public void Add(ProductOrder entity)
        {
            _marketDbContext.ProductOrders.Add(entity);
        }

        public ProductOrder FindById(int id)
        {
            return _marketDbContext.ProductOrders.FirstOrDefault(po => po.Id == id);
        }
    }
}
