using AutoService.Application.DTOs;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Controllers
{
    public class VozilaController : Controller
    {
        private readonly IRepository<Vozilo> _voziloRepository;
        private readonly IRepository<Korisnik> _korisnikRepository;
        private readonly IUnitOfWork _unitOfWork;

        public VozilaController(
            IRepository<Vozilo> voziloRepository,
            IRepository<Korisnik> korisnikRepository,
            IUnitOfWork unitOfWork)
        {
            _voziloRepository = voziloRepository;
            _korisnikRepository = korisnikRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? pretraga)
        {
            IQueryable<Vozilo> query = _voziloRepository
    .GetAll()
    .Include(v => v.Korisnik);
            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                query = query.Where(v =>
                    v.Marka.Contains(pretraga) ||
                    v.Model.Contains(pretraga) ||
                    v.Registracija.Contains(pretraga) ||
                    v.Korisnik.Ime.Contains(pretraga) ||
                    v.Korisnik.Prezime.Contains(pretraga));
            }

            var vozila = await query
                .OrderBy(v => v.Marka)
                .ThenBy(v => v.Model)
                .ToListAsync();

            ViewBag.Pretraga = pretraga;

            return View(vozila);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopuniKorisnike();

            return View(new VoziloDto
            {
                GodinaProizvodnje = DateTime.Now.Year
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoziloDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopuniKorisnike(dto.KorisnikId);
                return View(dto);
            }

            bool korisnikPostoji = await _korisnikRepository
                .GetAll()
                .AnyAsync(k => k.KorisnikId == dto.KorisnikId);

            if (!korisnikPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.KorisnikId),
                    "Izabrani korisnik ne postoji.");

                await PopuniKorisnike(dto.KorisnikId);

                return View(dto);
            }

            string registracija = dto.Registracija
                .Trim()
                .ToUpperInvariant();

            bool registracijaPostoji = await _voziloRepository
                .GetAll()
                .AnyAsync(v => v.Registracija == registracija);

            if (registracijaPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Registracija),
                    "Vozilo sa ovom registracijom već postoji.");

                await PopuniKorisnike(dto.KorisnikId);

                return View(dto);
            }

            var vozilo = new Vozilo
            {
                Marka = dto.Marka.Trim(),
                Model = dto.Model.Trim(),
                GodinaProizvodnje = dto.GodinaProizvodnje,
                Registracija = registracija,
                KorisnikId = dto.KorisnikId
            };

            await _voziloRepository.AddAsync(vozilo);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Vozilo je uspešno dodato.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var vozilo = await _voziloRepository.GetByIdAsync(id);

            if (vozilo == null)
            {
                return NotFound();
            }

            var dto = new VoziloDto
            {
                VoziloId = vozilo.VoziloId,
                Marka = vozilo.Marka,
                Model = vozilo.Model,
                GodinaProizvodnje = vozilo.GodinaProizvodnje,
                Registracija = vozilo.Registracija,
                KorisnikId = vozilo.KorisnikId
            };

            await PopuniKorisnike(dto.KorisnikId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VoziloDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopuniKorisnike(dto.KorisnikId);
                return View(dto);
            }

            var vozilo = await _voziloRepository
                .GetByIdAsync(dto.VoziloId);

            if (vozilo == null)
            {
                return NotFound();
            }

            bool korisnikPostoji = await _korisnikRepository
                .GetAll()
                .AnyAsync(k => k.KorisnikId == dto.KorisnikId);

            if (!korisnikPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.KorisnikId),
                    "Izabrani korisnik ne postoji.");

                await PopuniKorisnike(dto.KorisnikId);

                return View(dto);
            }

            string registracija = dto.Registracija
                .Trim()
                .ToUpperInvariant();

            bool registracijaPostoji = await _voziloRepository
                .GetAll()
                .AnyAsync(v =>
                    v.Registracija == registracija &&
                    v.VoziloId != dto.VoziloId);

            if (registracijaPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Registracija),
                    "Drugo vozilo već koristi ovu registraciju.");

                await PopuniKorisnike(dto.KorisnikId);

                return View(dto);
            }

            vozilo.Marka = dto.Marka.Trim();
            vozilo.Model = dto.Model.Trim();
            vozilo.GodinaProizvodnje = dto.GodinaProizvodnje;
            vozilo.Registracija = registracija;
            vozilo.KorisnikId = dto.KorisnikId;

            _voziloRepository.Update(vozilo);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Vozilo je uspešno izmenjeno.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vozilo = await _voziloRepository
                .GetAll()
                .Include(v => v.Korisnik)
                .Include(v => v.Termini)
                    .ThenInclude(t => t.ServisnaUsluga)
                .FirstOrDefaultAsync(v => v.VoziloId == id);

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vozilo = await _voziloRepository
                .GetAll()
                .Include(v => v.Korisnik)
                .Include(v => v.Termini)
                .FirstOrDefaultAsync(v => v.VoziloId == id);

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vozilo = await _voziloRepository
                .GetAll()
                .Include(v => v.Termini)
                .FirstOrDefaultAsync(v => v.VoziloId == id);

            if (vozilo == null)
            {
                return NotFound();
            }

            if (vozilo.Termini.Any())
            {
                TempData["Greska"] =
                    "Vozilo ne može biti obrisano jer ima evidentirane termine.";

                return RedirectToAction(nameof(Index));
            }

            _voziloRepository.Delete(vozilo);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Vozilo je uspešno obrisano.";

            return RedirectToAction(nameof(Index));
        }

        private async Task PopuniKorisnike(int? izabraniKorisnikId = null)
        {
            var korisnici = await _korisnikRepository
                .GetAll()
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .Select(k => new
                {
                    k.KorisnikId,
                    PunoIme = k.Ime + " " + k.Prezime
                })
                .ToListAsync();

            ViewBag.Korisnici = new SelectList(
                korisnici,
                "KorisnikId",
                "PunoIme",
                izabraniKorisnikId);
        }
    }
}
