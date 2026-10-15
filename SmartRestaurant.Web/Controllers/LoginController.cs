using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRestaurant.Web.Controllers
{
    public class LoginController : Controller
    {
        // Vista principal del Login (GET: /Login)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Acción provisional para recibir el formulario del login
        [HttpPost]
        public IActionResult Index(string email, string password)
        {
            // Aquí ira la logica de autenticacion más adelante con Identity.
            // Por ahora, solo redirige al Home o al Dashboard de manera simulada.
            return RedirectToAction("Index", "Home");
        }

        // Acción para cerrar sesion
        public IActionResult Logout()
        {
            return RedirectToAction("Index", "Login");
        }

        // Accion en caso de acceso denegado
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}