using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Serviseri.Queries;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Mediator.QueryHandlers
{
    public class GetServiseriQueryHandler : IRequestHandler<GetServiseriQuery, List<Serviser>>
    {
        private readonly IRepository<Serviser> _serviserRepository;

        public GetServiseriQueryHandler(IRepository<Serviser> serviserRepository)
        {
            _serviserRepository = serviserRepository;
        }

        public async Task<List<Serviser>> Handle(GetServiseriQuery request)
        {
            var query = _serviserRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Pretraga))
            {
                query = query.Where(s =>
                    s.Ime.Contains(request.Pretraga) ||
                    s.Prezime.Contains(request.Pretraga) ||
                    s.Specijalizacija.Contains(request.Pretraga));
            }

            if (request.Aktivan.HasValue)
            {
                query = query.Where(s => s.Aktivan == request.Aktivan.Value);
            }

            return await query
                .OrderBy(s => s.Prezime)
                .ThenBy(s => s.Ime)
                .ToListAsync();
        }
    }

    public class GetServiserEditQueryHandler : IRequestHandler<GetServiserEditQuery, ServiserDto?>
    {
        private readonly IRepository<Serviser> _serviserRepository;

        public GetServiserEditQueryHandler(IRepository<Serviser> serviserRepository)
        {
            _serviserRepository = serviserRepository;
        }

        public async Task<ServiserDto?> Handle(GetServiserEditQuery request)
        {
            var serviser = await _serviserRepository.GetByIdAsync(request.ServiserId);

            if (serviser == null)
            {
                return null;
            }

            return new ServiserDto
            {
                ServiserId = serviser.ServiserId,
                Ime = serviser.Ime,
                Prezime = serviser.Prezime,
                Specijalizacija = serviser.Specijalizacija,
                Aktivan = serviser.Aktivan,
                IsAdmin = serviser.IsAdmin,
                UserName = serviser.UserName,
                Email = serviser.Email
            };
        }
    }

    public class GetServiserDetailsQueryHandler : IRequestHandler<GetServiserDetailsQuery, Serviser?>
    {
        private readonly IRepository<Serviser> _serviserRepository;

        public GetServiserDetailsQueryHandler(IRepository<Serviser> serviserRepository)
        {
            _serviserRepository = serviserRepository;
        }

        public async Task<Serviser?> Handle(GetServiserDetailsQuery request)
        {
            return await _serviserRepository
                .GetAll()
                .Include(s => s.Termini)
                    .ThenInclude(t => t.Vozilo)
                .Include(s => s.Termini)
                    .ThenInclude(t => t.ServisnaUsluga)
                .FirstOrDefaultAsync(s => s.ServiserId == request.ServiserId);
        }
    }

    public class GetServiserDeleteQueryHandler : IRequestHandler<GetServiserDeleteQuery, Serviser?>
    {
        private readonly IRepository<Serviser> _serviserRepository;

        public GetServiserDeleteQueryHandler(IRepository<Serviser> serviserRepository)
        {
            _serviserRepository = serviserRepository;
        }

        public async Task<Serviser?> Handle(GetServiserDeleteQuery request)
        {
            return await _serviserRepository
                .GetAll()
                .Include(s => s.Termini)
                .FirstOrDefaultAsync(s => s.ServiserId == request.ServiserId);
        }
    }
}

