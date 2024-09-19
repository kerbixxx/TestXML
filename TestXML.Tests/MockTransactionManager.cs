using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestXML.Interfaces.Services;

namespace TestXML.Tests
{
    public class MockTransactionManager : ITransactionManager
    {
        public void BeginTransaction() { }
        public void CommitTransaction() { }
        public void RollbackTransaction() { }
        public void Dispose() { }
    }
}
