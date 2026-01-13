using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller para gestão e visualização de hotéis
    /// </summary>
    public class HotelController : Controller
    {
        private readonly AeroTejoContext _context;

        public HotelController(AeroTejoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista de hotéis disponíveis, com opção de filtrar por cidade
        /// </summary>
        public async Task<IActionResult> Index(string? cidade)
        {
            var query = _context.Hoteis.AsQueryable();

            // Filtrar por cidade se fornecido (normalmente vem do destino do voo)
            if (!string.IsNullOrWhiteSpace(cidade))
            {
                query = query.Where(h => h.Cidade.Contains(cidade));
                ViewBag.CidadeFiltro = cidade;
            }

            var hoteis = await query.OrderBy(h => h.Nome).ToListAsync();

            // Verificar se há um voo selecionado na sessão
            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            if (vooId.HasValue)
            {
                var voo = await _context.Voos.FindAsync(vooId.Value);
                ViewBag.VooSelecionado = voo;
            }

            return View(hoteis);
        }

        /// <summary>
        /// Detalhes de um hotel específico
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hotel = await _context.Hoteis.FirstOrDefaultAsync(m => m.Id == id);

            if (hotel == null)
            {
                return NotFound();
            }

            return View(hotel);
        }

        /// <summary>
        /// Selecionar hotel e prosseguir para configuração da reserva
        /// </summary>
        [HttpPost]
        public IActionResult SelectHotel(int? hotelId)
        {
            // Verificar se o utilizador está autenticado
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Precisa de fazer login para continuar com a reserva.";
                return RedirectToAction("Login", "Account");
            }

            // Guardar o hotel selecionado na sessão (pode ser null se não quiser hotel)
            if (hotelId.HasValue)
            {
                HttpContext.Session.SetInt32("SelectedHotelId", hotelId.Value);
            }

            // Redirecionar para a configuração da reserva
            return RedirectToAction("Configure", "Reserva");
        }
    }
}
