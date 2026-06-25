using AutoService.Application.Repositories;
using AutoService.Domain.Enums;
using AutoService.Domain.Models;
using AutoService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Infrastructure.Repositories
{
    public class TerminRepository : Repository<Termin>, ITerminRepository
    {
        public TerminRepository(AutoServiceDbContext context)
            : base(context)
        {
        }

        public IQueryable<Termin> GetAllWithDetails()
        {
            return DbSet
                .Include(t => t.Vozilo)
                    .ThenInclude(v => v.Korisnik)
                .Include(t => t.Serviser)
                .Include(t => t.ServisnaUsluga);
        }

        public async Task<bool> ServiserJeZauzetAsync(
            int serviserId,
            DateTime datumIVreme,
            int? terminId = null)
        {
            return await DbSet.AnyAsync(t =>
                t.ServiserId == serviserId &&
                t.DatumIVreme == datumIVreme &&
                t.Status != StatusTermina.Otkazan &&
                (!terminId.HasValue || t.TerminId != terminId.Value));
        }

        public async Task<bool> VoziloJeZauzetoAsync(
            int voziloId,
            DateTime datumIVreme,
            int? terminId = null)
        {
            return await DbSet.AnyAsync(t =>
                t.VoziloId == voziloId &&
                t.DatumIVreme == datumIVreme &&
                t.Status != StatusTermina.Otkazan &&
                (!terminId.HasValue || t.TerminId != terminId.Value));
        }
    }
}
