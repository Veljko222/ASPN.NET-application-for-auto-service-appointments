using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Domain.Models;

namespace AutoService.Application.Serviseri.Queries
{
    public class GetServiseriQuery : IRequest<List<Serviser>>
    {
        public GetServiseriQuery(string? pretraga, bool? aktivan)
        {
            Pretraga = pretraga;
            Aktivan = aktivan;
        }

        public string? Pretraga { get; }

        public bool? Aktivan { get; }
    }

    public class GetServiserEditQuery : IRequest<ServiserDto?>
    {
        public GetServiserEditQuery(int serviserId)
        {
            ServiserId = serviserId;
        }

        public int ServiserId { get; }
    }

    public class GetServiserDetailsQuery : IRequest<Serviser?>
    {
        public GetServiserDetailsQuery(int serviserId)
        {
            ServiserId = serviserId;
        }

        public int ServiserId { get; }
    }

    public class GetServiserDeleteQuery : IRequest<Serviser?>
    {
        public GetServiserDeleteQuery(int serviserId)
        {
            ServiserId = serviserId;
        }

        public int ServiserId { get; }
    }
}

