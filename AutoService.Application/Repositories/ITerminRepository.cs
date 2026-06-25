using AutoService.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoService.Application.Repositories
{
    public interface ITerminRepository : IRepository<Termin>
    {
        IQueryable<Termin> GetAllWithDetails();

        Task<bool> ServiserJeZauzetAsync(
            int serviserId,
            DateTime datumIVreme,
            int? terminId = null);

        Task<bool> VoziloJeZauzetoAsync(
            int voziloId,
            DateTime datumIVreme,
            int? terminId = null);
    }
}
