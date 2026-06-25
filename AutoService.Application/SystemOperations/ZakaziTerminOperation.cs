using AutoService.Application.Repositories;
using AutoService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.SystemOperations
{
    public class ZakaziTerminOperation : SystemOperationBase
    {
        private readonly ITerminRepository _terminRepository;

        private AutoService.Domain.Models.Termin _termin = null!;

        public ZakaziTerminOperation(
            ITerminRepository terminRepository,
            IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _terminRepository = terminRepository;
        }

        public async Task ExecuteAsync(
            AutoService.Domain.Models.Termin termin)
        {
            _termin = termin;

            await base.ExecuteAsync();
        }

        protected override void Validate()
        {
            if (_termin.DatumIVreme <= DateTime.Now)
            {
                throw new InvalidOperationException(
                    "Termin ne može biti zakazan u prošlosti.");
            }

            if (_termin.DatumIVreme.Hour < 8 ||
                _termin.DatumIVreme.Hour >= 18)
            {
                throw new InvalidOperationException(
                    "Termin mora biti zakazan između 08:00 i 18:00.");
            }

            if (_termin.DatumIVreme.Minute != 0)
            {
                throw new InvalidOperationException(
                    "Termin mora počinjati na pun sat.");
            }
        }

        protected override async Task ExecuteOperationAsync()
        {
            bool serviserZauzet =
                await _terminRepository.ServiserJeZauzetAsync(
                    _termin.ServiserId,
                    _termin.DatumIVreme);

            if (serviserZauzet)
            {
                throw new InvalidOperationException(
                    "Izabrani serviser već ima termin u to vreme.");
            }

            bool voziloZauzeto =
                await _terminRepository.VoziloJeZauzetoAsync(
                    _termin.VoziloId,
                    _termin.DatumIVreme);

            if (voziloZauzeto)
            {
                throw new InvalidOperationException(
                    "Izabrano vozilo već ima termin u to vreme.");
            }

            _termin.Status = StatusTermina.Zakazan;

            await _terminRepository.AddAsync(_termin);
        }
    }
}
