using AutoService.Application.DTOs;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Controllers
{
    public class ServisneUslugeController : Controller
    {
        private readonly IRepository<ServisnaUsluga> _uslugaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServisneUslugeController(
            IRepository<ServisnaUsluga> uslugaRepository,
            IUnitOfWork unitOfWork)
        {
            _uslugaRepository = uslugaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(
            string? pretraga,
            bool? aktivna)
        {
            IQueryable<ServisnaUsluga> query =
                _uslugaRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                query = query.Where(u =>
                    u.Naziv.Contains(pretraga) ||
                    u.Opis.Contains(pretraga));
            }

            if (aktivna.HasValue)
            {
                query = query.Where(u => u.Aktivna == aktivna.Value);
            }

            var usluge = await query
                .OrderBy(u => u.Naziv)
                .ToListAsync();

            ViewBag.Pretraga = pretraga;
            ViewBag.Aktivna = aktivna;

            return View(usluge);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ServisnaUslugaDto
            {
                Aktivna = true,
                TrajanjeUMinutima = 60
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServisnaUslugaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            bool nazivPostoji = await _uslugaRepository
                .GetAll()
                .AnyAsync(u => u.Naziv == dto.Naziv);

            if (nazivPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Naziv),
                    "Servisna usluga sa ovim nazivom već postoji.");

                return View(dto);
            }

            var usluga = new ServisnaUsluga
            {
                Naziv = dto.Naziv.Trim(),
                Opis = dto.Opis.Trim(),
                Cena = dto.Cena,
                TrajanjeUMinutima = dto.TrajanjeUMinutima,
                Aktivna = dto.Aktivna
            };

            await _uslugaRepository.AddAsync(usluga);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Servisna usluga je uspešno dodata.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var usluga = await _uslugaRepository.GetByIdAsync(id);

            if (usluga == null)
            {
                return NotFound();
            }

            var dto = new ServisnaUslugaDto
            {
                ServisnaUslugaId = usluga.ServisnaUslugaId,
                Naziv = usluga.Naziv,
                Opis = usluga.Opis,
                Cena = usluga.Cena,
                TrajanjeUMinutima = usluga.TrajanjeUMinutima,
                Aktivna = usluga.Aktivna
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServisnaUslugaDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var usluga = await _uslugaRepository
                .GetByIdAsync(dto.ServisnaUslugaId);

            if (usluga == null)
            {
                return NotFound();
            }

            bool nazivPostoji = await _uslugaRepository
                .GetAll()
                .AnyAsync(u =>
                    u.Naziv == dto.Naziv &&
                    u.ServisnaUslugaId != dto.ServisnaUslugaId);

            if (nazivPostoji)
            {
                ModelState.AddModelError(
                    nameof(dto.Naziv),
                    "Druga servisna usluga već koristi ovaj naziv.");

                return View(dto);
            }

            usluga.Naziv = dto.Naziv.Trim();
            usluga.Opis = dto.Opis.Trim();
            usluga.Cena = dto.Cena;
            usluga.TrajanjeUMinutima = dto.TrajanjeUMinutima;
            usluga.Aktivna = dto.Aktivna;

            _uslugaRepository.Update(usluga);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Servisna usluga je uspešno izmenjena.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var usluga = await _uslugaRepository
                .GetAll()
                .Include(u => u.Termini)
                    .ThenInclude(t => t.Vozilo)
                .FirstOrDefaultAsync(u => u.ServisnaUslugaId == id);

            if (usluga == null)
            {
                return NotFound();
            }

            return View(usluga);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usluga = await _uslugaRepository
                .GetAll()
                .Include(u => u.Termini)
                .FirstOrDefaultAsync(u => u.ServisnaUslugaId == id);

            if (usluga == null)
            {
                return NotFound();
            }

            return View(usluga);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usluga = await _uslugaRepository
                .GetAll()
                .Include(u => u.Termini)
                .FirstOrDefaultAsync(u => u.ServisnaUslugaId == id);

            if (usluga == null)
            {
                return NotFound();
            }

            if (usluga.Termini.Any())
            {
                usluga.Aktivna = false;

                _uslugaRepository.Update(usluga);
                await _unitOfWork.SaveChangesAsync();

                TempData["Uspeh"] =
                    "Usluga ima termine, pa je deaktivirana umesto obrisana.";

                return RedirectToAction(nameof(Index));
            }

            _uslugaRepository.Delete(usluga);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Servisna usluga je uspešno obrisana.";

            return RedirectToAction(nameof(Index));
        }
    }
}
