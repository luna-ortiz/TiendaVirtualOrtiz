using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TiendaVirtualOrtiz.Models;
using TiendaVirtualOrtiz.Data;

namespace TiendaVirtualOrtiz.Controllers
{
    public class LoginController : Controller
    {
        private readonly TiendaContext _context;
        public LoginController(TiendaContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(string correo, string clave) //recibe correo y clave
        {
            var usuario = _context.Usuarios //conecta con bd a la tabla de usuarios
                .FirstOrDefault(u => u.Correo == correo && u.Rol == clave); //envia y evalua con los datos de la tabla
            if (usuario != null)
            {
                HttpContext.Session.SetString("Usuario", usuario.Nombre);
                HttpContext.Session.SetString("Rol", usuario.Rol);

                return RedirectToAction("Index", "Producto");
            }
            ViewBag.Error = "Credenciales incorrectas";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
