using AutoService.Application.Repositories;
using AutoService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.SystemOperations
{
    public class OtkaziTerminOperation : SystemOperationBase
    {
        private readonly ITerminRepository _terminRepository;

        private int _terminId;

        public OtkaziTerminOperation(
            ITerminRepository terminRepository,
            IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _terminRepository = terminRepository;
        }

        public async Task ExecuteAsync(int terminId)
        {
            _terminId = terminId;

            await base.ExecuteAsync();
        }

        protected override async Task ExecuteOperationAsync()
        {
            var termin = await _terminRepository.GetByIdAsync(_terminId);

            if (termin == null)
            {
                throw new InvalidOperationException(
                    "Izabrani termin ne postoji.");
            }

            if (termin.Status == StatusTermina.Zavrsen)
            {
                throw new InvalidOperationException(
                    "Završen termin ne može biti otkazan.");
            }

            if (termin.Status == StatusTermina.Otkazan)
            {
                throw new InvalidOperationException(
                    "Termin je već otkazan.");
            }

            termin.Status = StatusTermina.Otkazan;

            _terminRepository.Update(termin);
        }
    }
}

