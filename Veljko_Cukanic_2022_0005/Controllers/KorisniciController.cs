using AutoService.Application.DTOs;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Controllers
{
    public class KorisniciController : Controller
    {
        private readonly IRepository<Korisnik> _korisnikRepository;
        private readonly IUnitOfWork _unitOfWork;

        public KorisniciController(
            IRepository<Korisnik> korisnikRepository,
            IUnitOfWork unitOfWork)
        {
            _korisnikRepository = korisnikRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? pretraga)
        {
            var query = _korisnikRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                query = query.Where(k =>
                    k.Ime.Contains(pretraga) ||
                    k.Prezime.Contains(pretraga) ||
                    k.Email.Contains(pretraga) ||
                    k.Telefon.Contains(pretraga));
            }

            var korisnici = await query
                .OrderBy(k => k.Prezime)
                .ThenBy(k => k.Ime)
                .ToListAsync();

            ViewBag.Pretraga = pretraga;

            return View(korisnici);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new KorisnikDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KorisnikDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool emailPostoji = await _korisnikRepository
                .GetAll()
                .AnyAsync(k => k.Email == dto.Email);

            if (emailPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Email),
                    "Korisnik sa ovim emailom već postoji.");

                return View(dto);
            }

            var korisnik = new Korisnik
            {
                Ime = dto.Ime,
                Prezime = dto.Prezime,
                Email = dto.Email,
                Telefon = dto.Telefon
            };

            await _korisnikRepository.AddAsync(korisnik);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Korisnik je uspešno dodat.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var korisnik = await _korisnikRepository.GetByIdAsync(id);

            if (korisnik == null)
            {
                return NotFound();
            }

            var dto = new KorisnikDto
            {
                KorisnikId = korisnik.KorisnikId,
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Email = korisnik.Email,
                Telefon = korisnik.Telefon
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(KorisnikDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var korisnik = await _korisnikRepository
                .GetByIdAsync(dto.KorisnikId);

            if (korisnik == null)
            {
                return NotFound();
            }

            bool emailPostoji = await _korisnikRepository
                .GetAll()
                .AnyAsync(k =>
                    k.Email == dto.Email &&
                    k.KorisnikId != dto.KorisnikId);

            if (emailPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Email),
                    "Drugi korisnik već koristi ovaj email.");

                return View(dto);
            }

            korisnik.Ime = dto.Ime;
            korisnik.Prezime = dto.Prezime;
            korisnik.Email = dto.Email;
            korisnik.Telefon = dto.Telefon;

            _korisnikRepository.Update(korisnik);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Korisnik je uspešno izmenjen.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var korisnik = await _korisnikRepository
                .GetAll()
                .Include(k => k.Vozila)
                .FirstOrDefaultAsync(k => k.KorisnikId == id);

            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var korisnik = await _korisnikRepository
                .GetAll()
                .Include(k => k.Vozila)
                .FirstOrDefaultAsync(k => k.KorisnikId == id);

            if (korisnik == null)
            {
                return NotFound();
            }

            return View(korisnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var korisnik = await _korisnikRepository
                .GetAll()
                .Include(k => k.Vozila)
                .FirstOrDefaultAsync(k => k.KorisnikId == id);

            if (korisnik == null)
            {
                return NotFound();
            }

            if (korisnik.Vozila.Any())
            {
                TempData["Greska"] =
                    "Korisnik ne može biti obrisan jer ima evidentirana vozila.";

                return RedirectToAction(nameof(Index));
            }

            _korisnikRepository.Delete(korisnik);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Korisnik je uspešno obrisan.";

            return RedirectToAction(nameof(Index));
        }
    }
}
