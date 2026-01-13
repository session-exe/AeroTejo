using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller para gestão e visualização de voos
    /// </summary>
    public class VooController : Controller
    {
        private readonly AeroTejoContext _context;

        public VooController(AeroTejoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de voos disponíveis, com opção de filtrar por destino
        /// </summary>
        public async Task<IActionResult> Index(string? destino)
        {
            // Query base para obter voos futuros
            var query = _context.Voos
                .Where(v => v.DataHora >= DateTime.Now)
                .OrderBy(v => v.DataHora)
                .AsQueryable();

            // Filtrar por destino se fornecido
            if (!string.IsNullOrWhiteSpace(destino))
            {
                query = query.Where(v => v.Destino.Contains(destino));
                ViewBag.DestinoFiltro = destino;
            }

            var voos = await query.ToListAsync();

            return View(voos);
        }

        /// <summary>
        /// Detalhes de um voo específico
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await _context.Voos
                .Include(v => v.Assentos)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        /// <summary>
        /// Selecionar voo e prosseguir para seleção de hotel
        /// </summary>
        [HttpPost]
        public IActionResult SelectFlight(int vooId)
        {
            // Guardar o voo selecionado na sessão
            HttpContext.Session.SetInt32("SelectedVooId", vooId);

            // Obter o destino do voo para filtrar hotéis
            var voo = _context.Voos.Find(vooId);
            if (voo == null)
            {
                TempData["ErrorMessage"] = "Voo não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // Redirecionar para a lista de hotéis no destino do voo
            return RedirectToAction("Index", "Hotel", new { cidade = voo.Destino });
        }
    }
}
