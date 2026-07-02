using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.Vlasnici.Queries
{
    public class GetVlasniciQuery : IRequest<List<Vlasnik>>
    {
        public GetVlasniciQuery(string? pretraga)
        {
            Pretraga = pretraga;
        }

        public string? Pretraga { get; }
    }

    public class GetVlasnikEditQuery : IRequest<VlasnikDto?>
    {
        public GetVlasnikEditQuery(int vlasnikId)
        {
            VlasnikId = vlasnikId;
        }

        public int VlasnikId { get; }
    }

    public class GetVlasnikDetailsQuery : IRequest<Vlasnik?>
    {
        public GetVlasnikDetailsQuery(int vlasnikId)
        {
            VlasnikId = vlasnikId;
        }

        public int VlasnikId { get; }
    }

    public class GetVlasnikDeleteQuery : IRequest<Vlasnik?>
    {
        public GetVlasnikDeleteQuery(int vlasnikId)
        {
            VlasnikId = vlasnikId;
        }

        public int VlasnikId { get; }
    }
}

