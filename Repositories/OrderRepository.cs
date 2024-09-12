using TestXML.Context;
using TestXML.Models;

namespace TestXML.Repositories
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly MarketDbContext _marketDbContext;

        public OrderRepository(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public void Add(Order entity)
        {
            _marketDbContext.Orders.Add(entity);
        }

        public Order FindById(int id)
        {
            return _marketDbContext.Orders.FirstOrDefault(o => o.Id == id);
        }

    }
}
