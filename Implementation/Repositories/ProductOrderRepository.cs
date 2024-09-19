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
    }
}
