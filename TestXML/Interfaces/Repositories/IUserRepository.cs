using TestXML.Models;

namespace TestXML.Interfaces.Repositories
{
    interface IUserRepository : IRepository<User>
    {
        User FindByEmail(string email);
    }
}
