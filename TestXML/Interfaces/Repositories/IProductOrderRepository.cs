using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    public interface IProductOrderRepository : IRepository<ProductOrder>
    {
        ProductOrder FindByProductIdAndOrderId(int orderNo, int productId);
        void RemoveRange(int orderId);
        IEnumerable<ProductOrder> GetAllByOrderId(int orderId);
    }
}
