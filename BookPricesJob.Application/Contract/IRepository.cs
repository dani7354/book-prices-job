using System.Collections.Generic;
using System.Linq.Expressions;

namespace BookPricesJob.Application.Contract;

public interface IRepository<T> where T : class
{
    Task<IList<T>> GetAll();
    Task<T?> GetById(int id);
    Task Add(T entity);
    void Update(T entity);
    void Delete(int id);
}
