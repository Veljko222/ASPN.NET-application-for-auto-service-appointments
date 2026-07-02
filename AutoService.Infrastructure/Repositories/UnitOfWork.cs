using AutoService.Application.Repositories;
using AutoService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AutoServiceDbContext _context;

        public UnitOfWork(AutoServiceDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

