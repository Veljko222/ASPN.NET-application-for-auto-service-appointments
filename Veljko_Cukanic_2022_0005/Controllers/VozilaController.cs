using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Vozila.Commands;
using AutoService.Application.Vozila.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AutoService.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class VozilaController : Controller
    {
        private readonly IMediator _mediator;

        public VozilaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? pretraga)
        {
            var vozila = await _mediator.Send(
                new GetVozilaQuery(pretraga, GetVlasnikId(), User.IsInRole("Admin")));

            ViewBag.Pretraga = pretraga;

            return View(vozila);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopuniVlasnike();

            return View(new VoziloDto
            {
                VlasnikId = GetVlasnikId() ?? 0,
                GodinaProizvodnje = DateTime.Now.Year
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VoziloDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopuniVlasnike(dto.VlasnikId);
                return View(dto);
            }

            try
            {
                await _mediator.Send(new CreateVoziloCommand(
                    dto,
                    GetVlasnikId(),
                    User.IsInRole("Admin")));
                TempData["Uspeh"] = "Vozilo je uspeÅ¡no dodato.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopuniVlasnike(dto.VlasnikId);

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetVoziloEditQuery(
                id,
                GetVlasnikId(),
                User.IsInRole("Admin")));

            if (dto == null)
            {
                return NotFound();
            }

            await PopuniVlasnike(dto.VlasnikId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VoziloDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopuniVlasnike(dto.VlasnikId);
                return View(dto);
            }

            try
            {
                await _mediator.Send(new UpdateVoziloCommand(
                    dto,
                    GetVlasnikId(),
                    User.IsInRole("Admin")));
                TempData["Uspeh"] = "Vozilo je uspeÅ¡no izmenjeno.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await PopuniVlasnike(dto.VlasnikId);

                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var vozilo = await _mediator.Send(
                new GetVoziloDetailsQuery(id, GetVlasnikId(), User.IsInRole("Admin")));

            if (vozilo == null)
            {
                return NotFound();
            }

            return View(vozilo);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vozilo = await _mediator.Send(
                new GetVoziloDeleteQuery(id, GetVlasnikId(), User.IsInRole("Admin")));

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
            try
            {
                await _mediator.Send(new DeleteVoziloCommand(
                    id,
                    GetVlasnikId(),
                    User.IsInRole("Admin")));
                TempData["Uspeh"] = "Vozilo je uspeÅ¡no obrisano.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopuniVlasnike(int? izabraniVlasnikId = null)
        {
            ViewBag.IsAdmin = User.IsInRole("Admin");

            if (!User.IsInRole("Admin"))
            {
                return;
            }

            var vlasnici = await _mediator.Send(
                new GetVlasniciZaVoziloQuery());

            ViewBag.Vlasnici = new SelectList(
                vlasnici.Select(k => new
                {
                    k.VlasnikId,
                    PunoIme = k.Ime + " " + k.Prezime
                }),
                "VlasnikId",
                "PunoIme",
                izabraniVlasnikId);
        }

        private int? GetVlasnikId()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int vlasnikId)
                ? vlasnikId
                : null;
        }
    }
}

