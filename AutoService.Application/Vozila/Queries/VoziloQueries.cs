using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.Vozila.Queries
{
    public class GetVozilaQuery : IRequest<List<Vozilo>>
    {
        public GetVozilaQuery(string? pretraga, int? vlasnikId, bool isAdmin)
        {
            Pretraga = pretraga;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public string? Pretraga { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetVoziloEditQuery : IRequest<VoziloDto?>
    {
        public GetVoziloEditQuery(int voziloId, int? vlasnikId, bool isAdmin)
        {
            VoziloId = voziloId;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int VoziloId { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetVoziloDetailsQuery : IRequest<Vozilo?>
    {
        public GetVoziloDetailsQuery(int voziloId, int? vlasnikId, bool isAdmin)
        {
            VoziloId = voziloId;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int VoziloId { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetVoziloDeleteQuery : IRequest<Vozilo?>
    {
        public GetVoziloDeleteQuery(int voziloId, int? vlasnikId, bool isAdmin)
        {
            VoziloId = voziloId;
            VlasnikId = vlasnikId;
            IsAdmin = isAdmin;
        }

        public int VoziloId { get; }

        public int? VlasnikId { get; }

        public bool IsAdmin { get; }
    }

    public class GetVlasniciZaVoziloQuery : IRequest<List<Vlasnik>>
    {
    }
}

