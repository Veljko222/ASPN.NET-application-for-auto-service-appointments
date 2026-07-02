using AutoService.Application.Mediator;
using AutoService.Application.SystemOperations;
using AutoService.Application.Termini.Commands;

namespace AutoService.Application.Termini.Handlers
{
    public class ZavrsiTerminCommandHandler : IRequestHandler<ZavrsiTerminCommand>
    {
        private readonly ZavrsiTerminOperation _zavrsiTerminOperation;

        public ZavrsiTerminCommandHandler(ZavrsiTerminOperation zavrsiTerminOperation)
        {
            _zavrsiTerminOperation = zavrsiTerminOperation;
        }

        public async Task Handle(ZavrsiTerminCommand request)
        {
            await _zavrsiTerminOperation.ExecuteAsync(request.TerminId);
        }
    }
}

