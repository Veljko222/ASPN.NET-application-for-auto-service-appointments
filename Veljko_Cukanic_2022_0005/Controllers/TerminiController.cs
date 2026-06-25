using AutoService.Application.DTOs;
using AutoService.Application.Repositories;
using AutoService.Application.SystemOperations;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Controllers
{
    public class TerminiController : Controller
    {
        private readonly ITerminRepository _terminRepository;
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;
        private readonly ZakaziTerminOperation _zakaziTerminOperation;
        private readonly OtkaziTerminOperation _otkaziTerminOperation;
        private readonly ZavrsiTerminOperation _zavrsiTerminOperation;

        public TerminiController(
         ITerminRepository terminRepository,
    IRepository<Vozilo> voziloRepository,
    IRepository<Serviser> serviserRepository,
    IRepository<ServisnaUsluga> uslugaRepository,
    ZakaziTerminOperation zakaziTerminOperation,
    OtkaziTerminOperation otkaziTerminOperation,
    ZavrsiTerminOperation zavrsiTerminOperation)
        {
            _terminRepository = terminRepository;
            _voziloRepository = voziloRepository;
            _serviserRepository = serviserRepository;
            _uslugaRepository = uslugaRepository;
            _zakaziTerminOperation = zakaziTerminOperation;
            _otkaziTerminOperation = otkaziTerminOperation;
            _zavrsiTerminOperation = zavrsiTerminOperation;
        }

        public async Task<IActionResult> Index()
        {
            var termini = await _terminRepository
                .GetAllWithDetails()
                .OrderBy(t => t.DatumIVreme)
                .ToListAsync();

            return View(termini);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopuniListe();

            var dto = new ZakaziTerminDto
            {
                DatumIVreme = DateTime.Now
                    .AddDays(1)
                    .Date
                    .AddHours(8)
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ZakaziTerminDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopuniListe();
                return View(dto);
            }

            var termin = new Termin
            {
                VoziloId = dto.VoziloId,
                ServiserId = dto.ServiserId,
                ServisnaUslugaId = dto.ServisnaUslugaId,
                DatumIVreme = dto.DatumIVreme,
                Napomena = dto.Napomena
            };

            try
            {
                await _zakaziTerminOperation.ExecuteAsync(termin);

                TempData["Uspeh"] = "Termin je uspešno zakazan.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                await PopuniListe();

                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Otkazi(int id)
        {
            try
            {
                await _otkaziTerminOperation.ExecuteAsync(id);

                TempData["Uspeh"] = "Termin je uspešno otkazan.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Zavrsi(int id)
        {
            try
            {
                await _zavrsiTerminOperation.ExecuteAsync(id);

                TempData["Uspeh"] = "Termin je uspešno označen kao završen.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopuniListe()
        {
            var vozila = await _voziloRepository
                .GetAll()
                .OrderBy(v => v.Marka)
                .ThenBy(v => v.Model)
                .ToListAsync();

            var serviseri = await _serviserRepository
                .GetAll()
                .Where(s => s.Aktivan)
                .OrderBy(s => s.Ime)
                .ThenBy(s => s.Prezime)
                .ToListAsync();

            var usluge = await _uslugaRepository
                .GetAll()
                .Where(u => u.Aktivna)
                .OrderBy(u => u.Naziv)
                .ToListAsync();

            ViewBag.Vozila = new SelectList(
                vozila,
                "VoziloId",
                "Registracija");

            ViewBag.Serviseri = new SelectList(
                serviseri.Select(s => new
                {
                    s.ServiserId,
                    PunoIme = $"{s.Ime} {s.Prezime}"
                }),
                "ServiserId",
                "PunoIme");

            ViewBag.Usluge = new SelectList(
                usluge.Select(u => new
                {
                    u.ServisnaUslugaId,
                    Prikaz = $"{u.Naziv} - {u.Cena:N2} RSD"
                }),
                "ServisnaUslugaId",
                "Prikaz");
        }
    }
}
