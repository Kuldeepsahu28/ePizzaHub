using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ePizzaHub.Repositories.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetById(object id);
        IEnumerable<TEntity> GetAll();
         void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(object id);
        void Remove(TEntity entity);
        int SaveChanges();
    }
}
