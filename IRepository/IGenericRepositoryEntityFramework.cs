using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IRepository
{
    public interface IGenericRepositoryEntityFramework<T>
        where T : class
    {
        //Task<PagedList<T>> FindPagedResultAsync(
        //    BaseQueryParameters queryParameters,
        //    Expression<Func<T, bool>> filterCondition = null
        //);
        void Create(T entity);
        Task CreateAsync(T entity);
        void Delete(T entity);
        void DeleteList(List<T> entities);
        Task<bool> ExistsAsync(Guid id);
        List<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        T FindOneById(Guid id);
        void Update(T entity);
        Task UpdateAsync(T entity);
        void UpdateList(List<T> entities);
    }
}
