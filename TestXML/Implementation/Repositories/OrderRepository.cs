using TestXML.Context;
using TestXML.Interfaces.Repositories;
using TestXML.Models;

namespace TestXML.Implementation.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly MarketDbContext _marketDbContext;

        public OrderRepository(MarketDbContext marketDbContext) : base(marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public Order FindByNo(int no)
        {
            return _marketDbContext.Orders.FirstOrDefault(o => o.No == no);
        }

        public void Update(Order entity)
        {
            _marketDbContext.Orders.Update(entity);
        }
    }
}
