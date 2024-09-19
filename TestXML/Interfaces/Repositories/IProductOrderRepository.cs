using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    interface IProductOrderRepository : IRepository<ProductOrder>
    {
        ProductOrder FindByProductIdAndOrderId(int orderId, int productId);
    }
}
