using AutoService.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.SystemOperations
{
    public abstract class SystemOperationBase
    {
        private readonly IUnitOfWork _unitOfWork;

        protected SystemOperationBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync()
        {
            Validate();

            await ExecuteOperationAsync();

            await _unitOfWork.SaveChangesAsync();
        }

        protected virtual void Validate()
        {
        }

        protected abstract Task ExecuteOperationAsync();
    }
}

