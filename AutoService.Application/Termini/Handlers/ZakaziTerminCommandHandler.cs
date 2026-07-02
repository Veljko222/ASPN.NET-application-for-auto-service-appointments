using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.SystemOperations;
using AutoService.Application.Termini.Commands;
using AutoService.Domain.Models;

namespace AutoService.Application.Termini.Handlers
{
    public class ZakaziTerminCommandHandler : IRequestHandler<ZakaziTerminCommand>
    {
        private readonly ZakaziTerminOperation _zakaziTerminOperation;
        private readonly IRepository<Vozilo> _voziloRepository;

        public ZakaziTerminCommandHandler(
            ZakaziTerminOperation zakaziTerminOperation,
            IRepository<Vozilo> voziloRepository)
        {
            _zakaziTerminOperation = zakaziTerminOperation;
            _voziloRepository = voziloRepository;
        }

        public async Task Handle(ZakaziTerminCommand request)
        {
            if (!request.IsAdmin)
            {
                bool voziloPripadaVlasniku = _voziloRepository
                    .GetAll()
                    .Any(v =>
                        v.VoziloId == request.Termin.VoziloId &&
                        v.VlasnikId == request.VlasnikId);

                if (!voziloPripadaVlasniku)
                {
                    throw new InvalidOperationException(
                        "Možete zakazati termin samo za svoje vozilo.");
                }
            }

            await _zakaziTerminOperation.ExecuteAsync(request.Termin);
        }
    }
}

