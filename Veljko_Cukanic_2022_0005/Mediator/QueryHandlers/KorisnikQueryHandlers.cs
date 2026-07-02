using AutoService.Application.DTOs;
using AutoService.Application.Vlasnici.Queries;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Mediator.QueryHandlers
{
    public class GetVlasniciQueryHandler : IRequestHandler<GetVlasniciQuery, List<Vlasnik>>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;

        public GetVlasniciQueryHandler(IRepository<Vlasnik> vlasnikRepository)
        {
            _vlasnikRepository = vlasnikRepository;
        }

        public async Task<List<Vlasnik>> Handle(GetVlasniciQuery request)
        {
            var query = _vlasnikRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(request.Pretraga))
            {
                query = query.Where(k =>
                    k.Ime.Contains(request.Pretraga) ||
                    k.Prezime.Contains(request.Pretraga) ||
                    k.Email.Contains(request.Pretraga) ||
                    k.Telefon.Contains(request.Pretraga));
            }

            return await query
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToListAsync();
        }
    }

    public class GetVlasnikEditQueryHandler : IRequestHandler<GetVlasnikEditQuery, VlasnikDto?>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;

        public GetVlasnikEditQueryHandler(IRepository<Vlasnik> vlasnikRepository)
        {
            _vlasnikRepository = vlasnikRepository;
        }

        public async Task<VlasnikDto?> Handle(GetVlasnikEditQuery request)
        {
            var vlasnik = await _vlasnikRepository.GetByIdAsync(request.VlasnikId);

            if (vlasnik == null)
            {
                return null;
            }

            return new VlasnikDto
            {
                VlasnikId = vlasnik.VlasnikId,
                Ime = vlasnik.Ime,
                Prezime = vlasnik.Prezime,
                Email = vlasnik.Email,
                Telefon = vlasnik.Telefon,
                UserName = vlasnik.UserName
            };
        }
    }

    public class GetVlasnikDetailsQueryHandler : IRequestHandler<GetVlasnikDetailsQuery, Vlasnik?>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;

        public GetVlasnikDetailsQueryHandler(IRepository<Vlasnik> vlasnikRepository)
        {
            _vlasnikRepository = vlasnikRepository;
        }

        public async Task<Vlasnik?> Handle(GetVlasnikDetailsQuery request)
        {
            return await _vlasnikRepository
                .GetAll()
                .Include(k => k.Vozila)
                .FirstOrDefaultAsync(k => k.VlasnikId == request.VlasnikId);
        }
    }

    public class GetVlasnikDeleteQueryHandler : IRequestHandler<GetVlasnikDeleteQuery, Vlasnik?>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;

        public GetVlasnikDeleteQueryHandler(IRepository<Vlasnik> vlasnikRepository)
        {
            _vlasnikRepository = vlasnikRepository;
        }

        public async Task<Vlasnik?> Handle(GetVlasnikDeleteQuery request)
        {
            return await _vlasnikRepository
                .GetAll()
                .Include(k => k.Vozila)
                .FirstOrDefaultAsync(k => k.VlasnikId == request.VlasnikId);
        }
    }
}

