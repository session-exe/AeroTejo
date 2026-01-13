using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller principal da aplicação
    /// </summary>
    public class HomeController : Controller
    {
        private readonly AeroTejoContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AeroTejoContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Página inicial com motor de busca de destinos
        /// </summary>
        public async Task<IActionResult> Index()
        {
            // Obter destinos populares para apresentar na página inicial
            var destinosPopulares = await _context.Voos
                .GroupBy(v => v.Destino)
                .Select(g => new { Destino = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(6)
                .Select(x => x.Destino)
                .ToListAsync();

            ViewBag.DestinosPopulares = destinosPopulares;

            return View();
        }

        /// <summary>
        /// Processa a pesquisa de destinos
        /// </summary>
        [HttpPost]
        public IActionResult Search(string destino)
        {
            if (string.IsNullOrWhiteSpace(destino))
            {
                TempData["ErrorMessage"] = "Por favor, introduza um destino.";
                return RedirectToAction(nameof(Index));
            }

            // Redirecionar para a lista de voos com o destino pesquisado
            return RedirectToAction("Index", "Voo", new { destino = destino });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
