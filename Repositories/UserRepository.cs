using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestXML.Context;
using TestXML.Models;

namespace TestXML.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly MarketDbContext _marketDbContext;

        public UserRepository(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public void Add(User entity)
        {
            _marketDbContext.Users.Add(entity);
        }

        public User FindById(int id)
        {
            return _marketDbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        public User FindByEmail(string email)
        {
            return _marketDbContext.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
