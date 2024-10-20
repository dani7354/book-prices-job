using System.Collections.Generic;
using System.Linq.Expressions;

namespace BookPricesJob.Application.Contract;

public interface IRepository<T> where T : class
{
    Task<IList<T>> GetAll();
    Task<T?> GetById(string id);
    Task<string> Add(T entity);
    Task Update(T entity);
    Task Delete(string id);
}
