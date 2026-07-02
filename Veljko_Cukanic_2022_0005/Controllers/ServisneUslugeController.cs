using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.ServisneUsluge.Commands;
using AutoService.Application.ServisneUsluge.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoService.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServisneUslugeController : Controller
    {
        private readonly IMediator _mediator;

        public ServisneUslugeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index(
            string? pretraga,
            bool? aktivna)
        {
            var usluge = await _mediator.Send(
                new GetServisneUslugeQuery(pretraga, aktivna));

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

            try
            {
                await _mediator.Send(new CreateServisnaUslugaCommand(dto));
                TempData["Uspeh"] = "Servisna usluga je uspeÅ¡no dodata.";

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
            var dto = await _mediator.Send(
                new GetServisnaUslugaEditQuery(id));

            if (dto == null)
            {
                return NotFound();
            }

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

            try
            {
                await _mediator.Send(new UpdateServisnaUslugaCommand(dto));
                TempData["Uspeh"] = "Servisna usluga je uspeÅ¡no izmenjena.";

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
            var usluga = await _mediator.Send(
                new GetServisnaUslugaDetailsQuery(id));

            if (usluga == null)
            {
                return NotFound();
            }

            return View(usluga);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var usluga = await _mediator.Send(
                new GetServisnaUslugaDeleteQuery(id));

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
            try
            {
                await _mediator.Send(new DeleteServisnaUslugaCommand(id));
                TempData["Uspeh"] = "Servisna usluga je uspeÅ¡no obraÄ‘ena.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

