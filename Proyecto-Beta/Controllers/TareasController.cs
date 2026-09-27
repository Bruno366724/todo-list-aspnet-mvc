using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Data;
using TodoListApp.Models;
using TodoListApp.ViewModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace TodoListApp.Controllers
{
    public class TareasController : Controller
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".gif" };
        private const long TamanioMaximoBytes = 5 * 1024 * 1024; // 5 MB

        public TareasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Tareas
        public async Task<IActionResult> Index()
        {
            var tareas = await _context.Tareas
                .OrderBy(t => t.Completada)
                .ThenByDescending(t => t.FechaCreacion)
                .ToListAsync();

            return View(tareas);
        }

        // GET: Tareas/Imagen/5
        public async Task<IActionResult> Imagen(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea?.ImagenDatos == null || tarea.ImagenContentType == null)
            {
                return NotFound();
            }

            return File(tarea.ImagenDatos, tarea.ImagenContentType);
        }

        // GET: Tareas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // GET: Tareas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TareaCreateViewModel modelo)
        {
            if (modelo.Imagen != null)
            {
                var errorImagen = ValidarImagen(modelo.Imagen);
                if (errorImagen != null)
                {
                    ModelState.AddModelError("Imagen", errorImagen);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var tarea = new Tarea
            {
                Titulo = modelo.Titulo,
                Descripcion = modelo.Descripcion,
                FechaCreacion = DateTime.Now,
                Completada = false
            };

            if (modelo.Imagen != null)
            {
                var (datos, contentType) = await LeerImagenAsync(modelo.Imagen);
                tarea.ImagenDatos = datos;
                tarea.ImagenContentType = contentType;
            }

            _context.Add(tarea);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Tareas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            var modelo = new TareaEditViewModel
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                TieneImagenActual = tarea.ImagenDatos != null
            };

            return View(modelo);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TareaEditViewModel modelo)
        {
            if (id != modelo.Id)
            {
                return NotFound();
            }

            if (modelo.Imagen != null)
            {
                var errorImagen = ValidarImagen(modelo.Imagen);
                if (errorImagen != null)
                {
                    ModelState.AddModelError("Imagen", errorImagen);
                }
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Titulo = modelo.Titulo;
            tarea.Descripcion = modelo.Descripcion;

            try
            {
                if (modelo.Imagen != null)
                {
                    var (datos, contentType) = await LeerImagenAsync(modelo.Imagen);
                    tarea.ImagenDatos = datos;
                    tarea.ImagenContentType = contentType;
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await TareaExisteAsync(tarea.Id))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Tareas/MarcarCompletada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarCompletada(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Completada = true;
            tarea.FechaRealizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { fechaRealizacion = tarea.FechaRealizacion?.ToString("dd/MM/yyyy HH:mm") });
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Tareas/DesmarcarCompletada/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DesmarcarCompletada(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Completada = false;
            tarea.FechaRealizacion = null;

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return NoContent();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Tareas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == id);

            if (tarea == null)
            {
                return NotFound();
            }

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id);

            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TareaExisteAsync(int id)
        {
            return await _context.Tareas.AnyAsync(t => t.Id == id);
        }

        private string? ValidarImagen(IFormFile imagen)
        {
            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();

            if (!ExtensionesPermitidas.Contains(extension))
            {
                return "Formato de imagen no permitido. Usá jpg, jpeg, png o gif.";
            }

            if (imagen.Length > TamanioMaximoBytes)
            {
                return "La imagen no puede superar los 5 MB.";
            }

            return null;
        }

        private const int AnchoMaximoPx = 800;
        private const int CalidadJpeg = 75;

        private static async Task<(byte[] Datos, string ContentType)> LeerImagenAsync(IFormFile imagen)
        {
            using var stream = imagen.OpenReadStream();
            using var imagenCargada = await Image.LoadAsync(stream);

            if (imagenCargada.Width > AnchoMaximoPx)
            {
                var nuevaAltura = (int)(imagenCargada.Height * (AnchoMaximoPx / (double)imagenCargada.Width));
                imagenCargada.Mutate(x => x.Resize(AnchoMaximoPx, nuevaAltura));
            }

            using var memoryStream = new MemoryStream();
            var encoder = new JpegEncoder { Quality = CalidadJpeg };
            await imagenCargada.SaveAsync(memoryStream, encoder);

            return (memoryStream.ToArray(), "image/jpeg");
        }
    }
}