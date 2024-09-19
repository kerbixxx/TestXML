using Microsoft.EntityFrameworkCore;
using TestXML.Context;
using TestXML.Interfaces.Services;

namespace TestXML.Implementation.Services
{
    public class TransactionManager : ITransactionManager
    {
        private readonly MarketDbContext _marketDbContext;

        public TransactionManager(MarketDbContext marketDbContext)
        {
            _marketDbContext = marketDbContext;
        }

        public void BeginTransaction()
        {
            _marketDbContext.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _marketDbContext.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _marketDbContext.Database.RollbackTransaction();
        }

        public void Dispose()
        {
            _marketDbContext?.Dispose();
        }
    }

}
