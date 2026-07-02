using AutoService.Application.Auth.Commands;
using AutoService.Application.Auth.Queries;
using AutoService.Application.DTOs;
using AutoService.Application.Mediator;
using AutoService.Web.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AutoService.Web.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var result = await _mediator.Send(new LoginUserQuery(dto));
                SetTokenCookie(result.Token);

                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterDto
            {
                GodinaProizvodnje = DateTime.Now.Year
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            try
            {
                var result = await _mediator.Send(new RegisterUserCommand(dto));
                SetTokenCookie(result.Token);

                return RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(dto);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(JwtAuthenticationHandler.CookieName);

            return RedirectToAction("Login", "Auth");
        }

        private void SetTokenCookie(string token)
        {
            Response.Cookies.Append(
                JwtAuthenticationHandler.CookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Strict
                });
        }
    }
}
