using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Vozila.Queries;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Mediator.QueryHandlers
{
    public class GetVozilaQueryHandler : IRequestHandler<GetVozilaQuery, List<Vozilo>>
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        public GetVozilaQueryHandler(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<List<Vozilo>> Handle(GetVozilaQuery request)
        {
            IQueryable<Vozilo> query = _voziloRepository
                .GetAll()
                .Include(v => v.Vlasnik);

            if (!request.IsAdmin)
            {
                query = query.Where(v => v.VlasnikId == request.VlasnikId);
            }

            if (!string.IsNullOrWhiteSpace(request.Pretraga))
            {
                query = query.Where(v =>
                    v.Marka.Contains(request.Pretraga) ||
                    v.Model.Contains(request.Pretraga) ||
                    v.Registracija.Contains(request.Pretraga) ||
                    v.Vlasnik.Ime.Contains(request.Pretraga) ||
                    v.Vlasnik.Prezime.Contains(request.Pretraga));
            }

            return await query
                .OrderBy(v => v.Marka)
                .ThenBy(v => v.Model)
                .ToListAsync();
        }
    }

    public class GetVoziloEditQueryHandler : IRequestHandler<GetVoziloEditQuery, VoziloDto?>
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        public GetVoziloEditQueryHandler(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<VoziloDto?> Handle(GetVoziloEditQuery request)
        {
            var vozilo = await _voziloRepository.GetByIdAsync(request.VoziloId);

            if (vozilo == null ||
                (!request.IsAdmin && vozilo.VlasnikId != request.VlasnikId))
            {
                return null;
            }

            return new VoziloDto
            {
                VoziloId = vozilo.VoziloId,
                Marka = vozilo.Marka,
                Model = vozilo.Model,
                GodinaProizvodnje = vozilo.GodinaProizvodnje,
                Registracija = vozilo.Registracija,
                VlasnikId = vozilo.VlasnikId
            };
        }
    }

    public class GetVoziloDetailsQueryHandler : IRequestHandler<GetVoziloDetailsQuery, Vozilo?>
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        public GetVoziloDetailsQueryHandler(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<Vozilo?> Handle(GetVoziloDetailsQuery request)
        {
            return await _voziloRepository
                .GetAll()
                .Include(v => v.Vlasnik)
                .Include(v => v.Termini)
                    .ThenInclude(t => t.ServisnaUsluga)
                .FirstOrDefaultAsync(v =>
                    v.VoziloId == request.VoziloId &&
                    (request.IsAdmin || v.VlasnikId == request.VlasnikId));
        }
    }

    public class GetVoziloDeleteQueryHandler : IRequestHandler<GetVoziloDeleteQuery, Vozilo?>
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        public GetVoziloDeleteQueryHandler(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<Vozilo?> Handle(GetVoziloDeleteQuery request)
        {
            return await _voziloRepository
                .GetAll()
                .Include(v => v.Vlasnik)
                .Include(v => v.Termini)
                .FirstOrDefaultAsync(v =>
                    v.VoziloId == request.VoziloId &&
                    (request.IsAdmin || v.VlasnikId == request.VlasnikId));
        }
    }

    public class GetVlasniciZaVoziloQueryHandler : IRequestHandler<GetVlasniciZaVoziloQuery, List<Vlasnik>>
    {
        private readonly IRepository<Vlasnik> _vlasnikRepository;

        public GetVlasniciZaVoziloQueryHandler(IRepository<Vlasnik> vlasnikRepository)
        {
            _vlasnikRepository = vlasnikRepository;
        }

        public async Task<List<Vlasnik>> Handle(GetVlasniciZaVoziloQuery request)
        {
            return await _vlasnikRepository
                .GetAll()
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToListAsync();
        }
    }
}

