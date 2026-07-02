using AutoService.Application.Mediator;

namespace AutoService.Application.Termini.Commands
{
    public class OtkaziTerminCommand : IRequest
    {
        public OtkaziTerminCommand(
            int terminId,
            int? vlasnikId,
            bool isAdmin)
        {
            TerminId = terminId;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int TerminId { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }
}

