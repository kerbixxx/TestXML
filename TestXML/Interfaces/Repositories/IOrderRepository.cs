using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Order FindByNo(int no);
        void Update(Order entity);
    }
}
