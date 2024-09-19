using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        Product FindByName(string name);
    }
}
