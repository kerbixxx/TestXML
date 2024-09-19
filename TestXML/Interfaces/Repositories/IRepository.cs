namespace TestXML.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        T FindById(int id);
        void Add(T entity);
        IEnumerable<T> GetAll();
        void Save();
    }
}