using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestXML.Repositories
{
    public interface IRepository<T> where T : class
    {
        T FindById(int id);
        void Add(T entity);
    }
}
