using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.ServisneUsluge.Queries;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Mediator.QueryHandlers
{
    public class GetServisneUslugeQueryHandler : IRequestHandler<GetServisneUslugeQuery, List<ServisnaUsluga>>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;

        public GetServisneUslugeQueryHandler(IRepository<ServisnaUsluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }

        public async Task<List<ServisnaUsluga>> Handle(GetServisneUslugeQuery request)
        {
            var query = _uslugaRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Pretraga))
            {
                query = query.Where(u =>
                    u.Naziv.Contains(request.Pretraga) ||
                    u.Opis.Contains(request.Pretraga));
            }

            if (request.Aktivna.HasValue)
            {
                query = query.Where(u => u.Aktivna == request.Aktivna.Value);
            }

            return await query
                .OrderBy(u => u.Naziv)
                .ToListAsync();
        }
    }

    public class GetServisnaUslugaEditQueryHandler : IRequestHandler<GetServisnaUslugaEditQuery, ServisnaUslugaDto?>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;

        public GetServisnaUslugaEditQueryHandler(IRepository<ServisnaUsluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }

        public async Task<ServisnaUslugaDto?> Handle(GetServisnaUslugaEditQuery request)
        {
            var usluga = await _uslugaRepository.GetByIdAsync(request.ServisnaUslugaId);

            if (usluga == null)
            {
                return null;
            }

            return new ServisnaUslugaDto
            {
                ServisnaUslugaId = usluga.ServisnaUslugaId,
                Naziv = usluga.Naziv,
                Opis = usluga.Opis,
                Cena = usluga.Cena,
                TrajanjeUMinutima = usluga.TrajanjeUMinutima,
                Aktivna = usluga.Aktivna
            };
        }
    }

    public class GetServisnaUslugaDetailsQueryHandler : IRequestHandler<GetServisnaUslugaDetailsQuery, ServisnaUsluga?>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;

        public GetServisnaUslugaDetailsQueryHandler(IRepository<ServisnaUsluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }

        public async Task<ServisnaUsluga?> Handle(GetServisnaUslugaDetailsQuery request)
        {
            return await _uslugaRepository
                .GetAll()
                .Include(u => u.Termini)
                    .ThenInclude(t => t.Vozilo)
                .FirstOrDefaultAsync(u =>
                    u.ServisnaUslugaId == request.ServisnaUslugaId);
        }
    }

    public class GetServisnaUslugaDeleteQueryHandler : IRequestHandler<GetServisnaUslugaDeleteQuery, ServisnaUsluga?>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;

        public GetServisnaUslugaDeleteQueryHandler(IRepository<ServisnaUsluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }

        public async Task<ServisnaUsluga?> Handle(GetServisnaUslugaDeleteQuery request)
        {
            return await _uslugaRepository
                .GetAll()
                .Include(u => u.Termini)
                .FirstOrDefaultAsync(u =>
                    u.ServisnaUslugaId == request.ServisnaUslugaId);
        }
    }
}

