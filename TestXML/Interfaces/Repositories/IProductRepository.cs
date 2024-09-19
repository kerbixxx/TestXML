using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    interface IProductRepository : IRepository<Product>
    {
        Product FindByName(string name);
    }
}
