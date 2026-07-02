using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.Termini.Commands
{
    public class ZakaziTerminCommand : IRequest
    {
        public ZakaziTerminCommand(
            Termin termin,
            int? vlasnikId,
            bool isAdmin)
        {
            Termin = termin;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public Termin Termin { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }
}

