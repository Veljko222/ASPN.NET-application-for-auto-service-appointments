using AutoService.Application.Mediator;
using AutoService.Application.Repositories;
using AutoService.Application.Termini.Queries;
using AutoService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Mediator.QueryHandlers
{
    public class GetTerminiQueryHandler : IRequestHandler<GetTerminiQuery, List<Termin>>
    {
        private readonly ITerminRepository _terminRepository;

        public GetTerminiQueryHandler(ITerminRepository terminRepository)
        {
            _terminRepository = terminRepository;
        }

        public async Task<List<Termin>> Handle(GetTerminiQuery request)
        {
            var query = _terminRepository.GetAllWithDetails();

            if (!request.IsAdmin)
            {
                query = query.Where(t =>
                    t.Vozilo.VlasnikId == request.VlasnikId);
            }

            return await query
                    .OrderBy(t => t.DatumIVreme)
                    .ToListAsync();
        }
    }

    public class GetVozilaZaTerminQueryHandler : IRequestHandler<GetVozilaZaTerminQuery, List<Vozilo>>
    {
        private readonly IRepository<Vozilo> _voziloRepository;

        public GetVozilaZaTerminQueryHandler(IRepository<Vozilo> voziloRepository)
        {
            _voziloRepository = voziloRepository;
        }

        public async Task<List<Vozilo>> Handle(GetVozilaZaTerminQuery request)
        {
            var query = _voziloRepository
                .GetAll()
                .AsQueryable();

            if (!request.IsAdmin)
            {
                query = query.Where(v => v.VlasnikId == request.VlasnikId);
            }

            return await query
                .OrderBy(v => v.Marka)
                .ThenBy(v => v.Model)
                .ToListAsync();
        }
    }

    public class GetServiseriZaTerminQueryHandler : IRequestHandler<GetServiseriZaTerminQuery, List<Serviser>>
    {
        private readonly IRepository<Serviser> _serviserRepository;

        public GetServiseriZaTerminQueryHandler(IRepository<Serviser> serviserRepository)
        {
            _serviserRepository = serviserRepository;
        }

        public async Task<List<Serviser>> Handle(GetServiseriZaTerminQuery request)
        {
            return await _serviserRepository
                .GetAll()
                .Where(s => s.Aktivan)
                .OrderBy(s => s.Ime)
                .ThenBy(s => s.Prezime)
                .ToListAsync();
        }
    }

    public class GetUslugeZaTerminQueryHandler : IRequestHandler<GetUslugeZaTerminQuery, List<ServisnaUsluga>>
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;

        public GetUslugeZaTerminQueryHandler(IRepository<ServisnaUsluga> uslugaRepository)
        {
            _uslugaRepository = uslugaRepository;
        }

        public async Task<List<ServisnaUsluga>> Handle(GetUslugeZaTerminQuery request)
        {
            return await _uslugaRepository
                .GetAll()
                .Where(u => u.Aktivna)
                .OrderBy(u => u.Naziv)
                .ToListAsync();
        }
    }
}

