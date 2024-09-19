using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    interface IOrderRepository : IRepository<Order>
    {
        Order FindByNo(int no);
        void Update(Order entity);
    }
}
