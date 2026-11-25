using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartRestaurant.Application.DTOs.Auth;
using SmartRestaurant.Application.Interfaces.Services;
using SmartRestaurant.Web.ViewModels;

namespace SmartRestaurant.Web.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IValidator<LoginDto> _loginValidator;

        public LoginController(IAuthService authService, IValidator<LoginDto> loginValidator)
        {
            _authService = authService;
            _loginValidator = loginValidator;
        }

        [HttpGet]
        public IActionResult Index(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                model.Password = string.Empty;
                return View(model);
            }

            var dto = new LoginDto { Email = model.Email, Password = model.Password };

            var validation = await _loginValidator.ValidateAsync(dto, ct);
            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                model.Password = string.Empty;
                return View(model);
            }

            var result = await _authService.ValidateCredentialsAsync(dto, ct);
            if (!result.Succeeded || result.User is null)
            {
                ModelState.AddModelError(string.Empty, GetFailureMessage(result.FailureReason));
                model.Password = string.Empty;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, result.User.Id),
                new(ClaimTypes.Email, result.User.Email),
                new(ClaimTypes.Name, result.User.FullName)
            };
            claims.AddRange(result.User.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return LocalRedirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private static string GetFailureMessage(AuthFailureReason reason) =>
            reason switch
            {
                AuthFailureReason.LockedOut => "Cuenta bloqueada temporalmente por demasiados intentos. Intenta de nuevo en unos minutos.",
                AuthFailureReason.Inactive => "Tu cuenta está desactivada. Contacta al administrador.",
                _ => "Correo o contraseña incorrectos."
            };
    }
}