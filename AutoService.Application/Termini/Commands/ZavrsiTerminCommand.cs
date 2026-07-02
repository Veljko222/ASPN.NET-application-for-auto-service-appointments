using AutoService.Application.Mediator;

namespace AutoService.Application.Termini.Commands
{
    public class ZavrsiTerminCommand : IRequest
    {
        public ZavrsiTerminCommand(int terminId)
        {
            TerminId = terminId;
        }

        public int TerminId { get; }
    }
}

