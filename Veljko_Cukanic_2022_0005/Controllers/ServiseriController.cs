using AutoService.Application.DTOs;
using AutoService.Application.Repositories;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoService.Web.Controllers
{
    public class ServiseriController : Controller
    {
        private readonly IRepository<Serviser> _serviserRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiseriController(
            IRepository<Serviser> serviserRepository,
            IUnitOfWork unitOfWork)
        {
            _serviserRepository = serviserRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(
            string? pretraga,
            bool? aktivan)
        {
            IQueryable<Serviser> query = _serviserRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(pretraga))
            {
                query = query.Where(s =>
                    s.Ime.Contains(pretraga) ||
                    s.Prezime.Contains(pretraga) ||
                    s.Specijalizacija.Contains(pretraga));
            }

            if (aktivan.HasValue)
            {
                query = query.Where(s => s.Aktivan == aktivan.Value);
            }

            var serviseri = await query
                .OrderBy(s => s.Prezime)
                .ThenBy(s => s.Ime)
                .ToListAsync();

            ViewBag.Pretraga = pretraga;
            ViewBag.Aktivan = aktivan;

            return View(serviseri);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new ServiserDto
            {
                Aktivan = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var serviser = new Serviser
            {
                Ime = dto.Ime.Trim(),
                Prezime = dto.Prezime.Trim(),
                Specijalizacija = dto.Specijalizacija.Trim(),
                Aktivan = dto.Aktivan
            };

            await _serviserRepository.AddAsync(serviser);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Serviser je uspešno dodat.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var serviser = await _serviserRepository.GetByIdAsync(id);

            if (serviser == null)
            {
                return NotFound();
            }

            var dto = new ServiserDto
            {
                ServiserId = serviser.ServiserId,
                Ime = serviser.Ime,
                Prezime = serviser.Prezime,
                Specijalizacija = serviser.Specijalizacija,
                Aktivan = serviser.Aktivan
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ServiserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var serviser = await _serviserRepository
                .GetByIdAsync(dto.ServiserId);

            if (serviser == null)
            {
                return NotFound();
            }

            serviser.Ime = dto.Ime.Trim();
            serviser.Prezime = dto.Prezime.Trim();
            serviser.Specijalizacija = dto.Specijalizacija.Trim();
            serviser.Aktivan = dto.Aktivan;

            _serviserRepository.Update(serviser);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Serviser je uspešno izmenjen.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var serviser = await _serviserRepository
                .GetAll()
                .Include(s => s.Termini)
                    .ThenInclude(t => t.Vozilo)
                .Include(s => s.Termini)
                    .ThenInclude(t => t.ServisnaUsluga)
                .FirstOrDefaultAsync(s => s.ServiserId == id);

            if (serviser == null)
            {
                return NotFound();
            }

            return View(serviser);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var serviser = await _serviserRepository
                .GetAll()
                .Include(s => s.Termini)
                .FirstOrDefaultAsync(s => s.ServiserId == id);

            if (serviser == null)
            {
                return NotFound();
            }

            return View(serviser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var serviser = await _serviserRepository
                .GetAll()
                .Include(s => s.Termini)
                .FirstOrDefaultAsync(s => s.ServiserId == id);

            if (serviser == null)
            {
                return NotFound();
            }

            if (serviser.Termini.Any())
            {
                serviser.Aktivan = false;

                _serviserRepository.Update(serviser);
                await _unitOfWork.SaveChangesAsync();

                TempData["Uspeh"] =
                    "Serviser ima termine, pa je deaktiviran umesto obrisan.";

                return RedirectToAction(nameof(Index));
            }

            _serviserRepository.Delete(serviser);
            await _unitOfWork.SaveChangesAsync();

            TempData["Uspeh"] = "Serviser je uspešno obrisan.";

            return RedirectToAction(nameof(Index));
        }
    }
}
