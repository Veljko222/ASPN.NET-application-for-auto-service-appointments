using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Serviseri.Commands;
using AutoService.Application.Serviseri.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoService.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiseriController : Controller
    {
        private readonly IMediator _mediator;

        public ServiseriController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(
            string? pretraga,
            bool? aktivan)
        {
            var serviseri = await _mediator.Send(
                new GetServiseriQuery(pretraga, aktivan));

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

            await _mediator.Send(new CreateServiserCommand(dto));
            TempData["Uspeh"] = "Serviser je uspeÅ¡no dodat.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetServiserEditQuery(id));

            if (dto == null)
            {
                return NotFound();
            }

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

            try
            {
                await _mediator.Send(new UpdateServiserCommand(dto));
                TempData["Uspeh"] = "Serviser je uspeÅ¡no izmenjen.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var serviser = await _mediator.Send(
                new GetServiserDetailsQuery(id));

            if (serviser == null)
            {
                return NotFound();
            }

            return View(serviser);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var serviser = await _mediator.Send(
                new GetServiserDeleteQuery(id));

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
            try
            {
                await _mediator.Send(new DeleteServiserCommand(id));
                TempData["Uspeh"] = "Serviser je uspeÅ¡no obraÄ‘en.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

