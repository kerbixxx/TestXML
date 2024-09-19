namespace TestXML.Interfaces.Repositories
{
    interface IRepository<T> where T : class
    {
        T FindById(int id);
        void Add(T entity);
        void Save();
    }
}