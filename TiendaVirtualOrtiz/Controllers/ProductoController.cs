using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TiendaVirtualOrtiz.Data;
using TiendaVirtualOrtiz.Models;
using System.Text.Json;

namespace TiendaVirtualOrtiz.Controllers
{
    public class ProductoController : Controller
    {
        private readonly TiendaContext _context;

        public ProductoController(TiendaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var productos = _context.Productos
                .Include(p => p.Categoria)
                .ToList();

            return View(productos);
        }

        //Guardar producto

        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.Categorias = _context.Categorias.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Producto producto, IFormFile imagen)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (imagen != null)
            {
                var ruta = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/images", imagen.FileName);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }

                producto.ImagenUrl = "/images/" + imagen.FileName;
            }

            _context.Productos.Add(producto);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //Formulario editar
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var producto = _context.Productos.Find(id);
            ViewBag.Categorias = _context.Categorias.ToList();

            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(Producto producto, IFormFile imagen)
        {
            var productoDB = _context.Productos.Find(producto.Id);
            if (productoDB == null)

                return NotFound();
                //Actualizar datos normales
            productoDB.Nombre = producto.Nombre;
            productoDB.Precio = producto.Precio;
            productoDB.Stock = producto.Stock;
            productoDB.CategoriaId = producto.CategoriaId;

            if(imagen != null)
            {
                var carpeta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                if(!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }
                var ruta = Path.Combine(carpeta, imagen.FileName);

                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    imagen.CopyTo(stream);
                }
                productoDB.ImagenUrl = "/images/" + imagen.FileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //Eliminar producto
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var rol = HttpContext.Session.GetString("Rol");

            //SOLO ADMIN PUEDE ELIMINAR
            if (rol != "admin")
            {
                return RedirectToAction("Index");
            }

            var producto = _context.Productos.Find(id);

            _context.Productos.Remove(producto);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        //agregar carrito
        public IActionResult AgregarCarrito(int id, int cantidad)
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            List<CarritoItem> carrito;
            if (carritoJson == null)
            {
                carrito = new List<CarritoItem>();
            }
            else
            {
                carrito = JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson);
            }

            var item = carrito.FirstOrDefault(p => p.ProductoId == id);

            if (item == null)
            {
                item.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new CarritoItem
                {
                    ProductoId = id,
                    Cantidad = cantidad
                });
            }
            HttpContext.Session.SetString("Carrito", JsonSerializer.Serialize(carrito));
            return RedirectToAction("Index");
        }

        //ver lista de carrito
        public IActionResult Carrito()
        {
            var carritoJson = HttpContext.Session.GetString("Carrito");
            List<CarritoItem> carrito;

            if (carritoJson == null)
                carrito = new List<CarritoItem>();
            else
                carrito = JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson);

            var productos = new List<(Producto producto, int cantidad)>();

            foreach (var item in carrito)
            {
                var producto = _context.Productos.Find(item.ProductoId);

                if (producto != null)
                {
                    productos.Add((producto.item.Cantidad));
                }
            }

            return View(productos);
        }

        public IActionResult Comprar()
        {
            var carritoJson = HttpContext.Session.GetString("carrito");

            if (carritoJson == null)
                return RedirectToAction("Index");

            var carrito = JsonSerializer.Deserialize<List<CarritoItem>>(carritoJson);

            foreach (var item in carrito)
            {
                var producto = _context.Productos.Find(item.ProductoId);

                if (producto != null)
                {
                    if (producto.Stock >= item.Cantidad)
                    {
                        producto.Stock -= item.Cantidad;
                    }
                }
            }

            _context.SaveChanges();

            HttpContext.Session.Remove("Carrito");

            return RedirectToAction("Index");
        }


    }
}
