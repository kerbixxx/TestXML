using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestXML.Context;
using TestXML.Interfaces.Repositories;
using TestXML.Models;

namespace TestXML.Implementation.Repositories
{
    public class UserRepository : Repository<User>,IUserRepository
    {
        private readonly MarketDbContext _marketDbContext;

        public UserRepository(MarketDbContext marketDbContext) : base(marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public User FindByEmail(string email)
        {
            return _marketDbContext.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
