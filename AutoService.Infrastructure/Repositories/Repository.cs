using AutoService.Application.Repositories;
using AutoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AutoServiceDbContext Context;
        protected readonly DbSet<T> DbSet;

        public Repository(AutoServiceDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await DbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
    }
}

