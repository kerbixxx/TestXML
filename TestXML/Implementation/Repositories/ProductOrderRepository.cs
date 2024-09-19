using TestXML.Context;
using TestXML.Interfaces.Repositories;
using TestXML.Models;

namespace TestXML.Implementation.Repositories
{
    public class ProductOrderRepository : Repository<ProductOrder>, IProductOrderRepository
    {
        private readonly MarketDbContext _marketDbContext;

        public ProductOrderRepository(MarketDbContext marketDbContext) : base(marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public ProductOrder FindByProductIdAndOrderId(int orderId, int productId)
        {

            return _marketDbContext.ProductOrders.FirstOrDefault(po => po.OrderId == orderId && po.ProductId == productId);
        }

        public IEnumerable<ProductOrder> GetAllByOrderId(int orderId)
        {
            return _marketDbContext.ProductOrders.Where(po=>po.OrderId == orderId);
        }

        public void RemoveRange(int orderId)
        {
            _marketDbContext.ProductOrders.RemoveRange(_marketDbContext.ProductOrders.Where(po => po.OrderId == orderId));
            Save();
        }
    }
}
