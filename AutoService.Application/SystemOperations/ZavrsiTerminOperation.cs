using AutoService.Application.Repositories;
using AutoService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.SystemOperations
{
    public class ZavrsiTerminOperation : SystemOperationBase
    {
        private readonly ITerminRepository _terminRepository;

        private int _terminId;

        public ZavrsiTerminOperation(
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

            if (termin.Status == StatusTermina.Otkazan)
            {
                throw new InvalidOperationException(
                    "Otkazan termin ne može biti završen.");
            }

            if (termin.Status == StatusTermina.Zavrsen)
            {
                throw new InvalidOperationException(
                    "Termin je već završen.");
            }

            termin.Status = StatusTermina.Zavrsen;

            _terminRepository.Update(termin);
        }
    }
}

