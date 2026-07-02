using AutoService.Application.DTOs;
using AutoService.Application.Vlasnici.Commands;
using AutoService.Application.Vlasnici.Queries;
using AutoService.Application.Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoService.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class VlasniciController : Controller
    {
        private readonly IMediator _mediator;

        public VlasniciController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(string? pretraga)
        {
            var vlasnici = await _mediator.Send(
                new GetVlasniciQuery(pretraga));

            ViewBag.Pretraga = pretraga;

            return View(vlasnici);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new VlasnikDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VlasnikDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _mediator.Send(new CreateVlasnikCommand(dto));
                TempData["Uspeh"] = "Vlasnik je uspeÅ¡no dodat.";

                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _mediator.Send(new GetVlasnikEditQuery(id));

            if (dto == null)
            {
                return NotFound();
            }

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VlasnikDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                await _mediator.Send(new UpdateVlasnikCommand(dto));
                TempData["Uspeh"] = "Vlasnik je uspeÅ¡no izmenjen.";

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
            var vlasnik = await _mediator.Send(
                new GetVlasnikDetailsQuery(id));

            if (vlasnik == null)
            {
                return NotFound();
            }

            return View(vlasnik);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var vlasnik = await _mediator.Send(
                new GetVlasnikDeleteQuery(id));

            if (vlasnik == null)
            {
                return NotFound();
            }

            return View(vlasnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _mediator.Send(new DeleteVlasnikCommand(id));
                TempData["Uspeh"] = "Vlasnik je uspeÅ¡no obrisan.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

