using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User FindByEmail(string email);
    }
}
