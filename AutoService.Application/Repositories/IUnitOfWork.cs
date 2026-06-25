using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
