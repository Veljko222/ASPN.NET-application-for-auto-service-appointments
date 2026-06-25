using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
