using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.ServisneUsluge.Queries
{
    public class GetServisneUslugeQuery : IRequest<List<ServisnaUsluga>>
    {
        public GetServisneUslugeQuery(string? pretraga, bool? aktivna)
        {
            Pretraga = pretraga;
            Aktivna = aktivna;
        }

        public string? Pretraga { get; }

        public bool? Aktivna { get; }
    }

    public class GetServisnaUslugaEditQuery : IRequest<ServisnaUslugaDto?>
    {
        public GetServisnaUslugaEditQuery(int servisnaUslugaId)
        {
            ServisnaUslugaId = servisnaUslugaId;
        }

        public int ServisnaUslugaId { get; }
    }

    public class GetServisnaUslugaDetailsQuery : IRequest<ServisnaUsluga?>
    {
        public GetServisnaUslugaDetailsQuery(int servisnaUslugaId)
        {
            ServisnaUslugaId = servisnaUslugaId;
        }

        public int ServisnaUslugaId { get; }
    }

    public class GetServisnaUslugaDeleteQuery : IRequest<ServisnaUsluga?>
    {
        public GetServisnaUslugaDeleteQuery(int servisnaUslugaId)
        {
            ServisnaUslugaId = servisnaUslugaId;
        }

        public int ServisnaUslugaId { get; }
    }
}

