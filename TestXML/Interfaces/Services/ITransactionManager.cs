namespace TestXML.Interfaces.Services
{
    public interface ITransactionManager
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        void Dispose();
    }
}
