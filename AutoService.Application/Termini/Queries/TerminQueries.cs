using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.Termini.Queries
{
    public class GetTerminiQuery : IRequest<List<Termin>>
    {
        public GetTerminiQuery(int? vlasnikId, bool isAdmin)
        {
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetVozilaZaTerminQuery : IRequest<List<Vozilo>>
    {
        public GetVozilaZaTerminQuery(int? vlasnikId, bool isAdmin)
        {
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetServiseriZaTerminQuery : IRequest<List<Serviser>>
    {
    }

    public class GetUslugeZaTerminQuery : IRequest<List<ServisnaUsluga>>
    {
    }
}

