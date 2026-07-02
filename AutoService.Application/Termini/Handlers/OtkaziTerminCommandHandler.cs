using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.SystemOperations;
using AutoService.Application.Termini.Commands;
using AutoService.Domain.Models;

namespace AutoService.Application.Termini.Handlers
{
    public class OtkaziTerminCommandHandler : IRequestHandler<OtkaziTerminCommand>
    {
        private readonly OtkaziTerminOperation _otkaziTerminOperation;
        private readonly IRepository<Termin> _terminRepository;

        public OtkaziTerminCommandHandler(
            OtkaziTerminOperation otkaziTerminOperation,
            IRepository<Termin> terminRepository)
        {
            _otkaziTerminOperation = otkaziTerminOperation;
            _terminRepository = terminRepository;
        }

        public async Task Handle(OtkaziTerminCommand request)
        {
            if (!request.IsAdmin)
            {
                bool terminPripadaVlasniku = _terminRepository
                    .GetAll()
                    .Any(t =>
                        t.TerminId == request.TerminId &&
                        t.Vozilo.VlasnikId == request.VlasnikId);

                if (!terminPripadaVlasniku)
                {
                    throw new InvalidOperationException(
                        "Možete otkazati samo svoj termin.");
                }
            }

            await _otkaziTerminOperation.ExecuteAsync(request.TerminId);
        }
    }
}

