using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Application.Termini.Commands;
using AutoService.Application.Termini.Queries;
using AutoService.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AutoService.Web.Controllers
{
    [Authorize(Roles = "Admin,User")]
    public class TerminiController : Controller
    {
        private readonly IMediator _mediator;

        public TerminiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var termini = await _mediator.Send(
                new GetTerminiQuery(GetVlasnikId(), User.IsInRole("Admin")));

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
                await _mediator.Send(new ZakaziTerminCommand(
                    termin,
                    GetVlasnikId(),
                    User.IsInRole("Admin")));
                TempData["Uspeh"] = "Termin je uspeÅ¡no zakazan.";

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
                await _mediator.Send(new OtkaziTerminCommand(
                    id,
                    GetVlasnikId(),
                    User.IsInRole("Admin")));
                TempData["Uspeh"] = "Termin je uspeÅ¡no otkazan.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Zavrsi(int id)
        {
            try
            {
                await _mediator.Send(new ZavrsiTerminCommand(id));
                TempData["Uspeh"] = "Termin je uspeÅ¡no oznaÄen kao zavrÅ¡en.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Greska"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopuniListe()
        {
            var vozila = await _mediator.Send(
                new GetVozilaZaTerminQuery(GetVlasnikId(), User.IsInRole("Admin")));
            var serviseri = await _mediator.Send(new GetServiseriZaTerminQuery());
            var usluge = await _mediator.Send(new GetUslugeZaTerminQuery());

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

        private int? GetVlasnikId()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(id, out int vlasnikId)
                ? vlasnikId
                : null;
        }
    }
}

