using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller para o painel administrativo
    /// Apenas acessível para utilizadores com papel "Administrador"
    /// </summary>
    public class AdminController : Controller
    {
        private readonly AeroTejoContext _context;

        public AdminController(AeroTejoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se o utilizador é administrador
        /// </summary>
        private bool IsAdmin()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            return userRole == "Administrador";
        }

        /// <summary>
        /// Dashboard principal do administrador com estatísticas
        /// </summary>
        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Acesso negado. Apenas administradores.";
                return RedirectToAction("Index", "Home");
            }

            // Estatísticas gerais
            ViewBag.TotalVoos = await _context.Voos.CountAsync();
            ViewBag.TotalHoteis = await _context.Hoteis.CountAsync();
            ViewBag.TotalReservas = await _context.Reservas.CountAsync();
            ViewBag.TotalUtilizadores = await _context.Users.CountAsync();
            ViewBag.ReceitaTotal = await _context.Reservas.SumAsync(r => r.ValorTotal);

            // Destinos mais populares
            var destinosPopulares = await _context.Reservas
                .Include(r => r.Voo)
                .GroupBy(r => r.Voo!.Destino)
                .Select(g => new { Destino = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            ViewBag.DestinosPopulares = destinosPopulares;

            // Reservas por mês (últimos 12 meses)
            var dataInicio = DateTime.Now.AddMonths(-12);
            var reservasPorMes = await _context.Reservas
                .Where(r => r.DataReserva >= dataInicio)
                .GroupBy(r => new { r.DataReserva.Year, r.DataReserva.Month })
                .Select(g => new
                {
                    Mes = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Count = g.Count(),
                    Receita = g.Sum(r => r.ValorTotal)
                })
                .OrderBy(x => x.Mes)
                .ToListAsync();

            ViewBag.ReservasPorMes = reservasPorMes;

            return View();
        }

        // ==================== CRUD DE VOOS ====================

        /// <summary>
        /// Lista de voos para gestão
        /// </summary>
        public async Task<IActionResult> Voos()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var voos = await _context.Voos.OrderBy(v => v.DataHora).ToListAsync();
            return View(voos);
        }

        /// <summary>
        /// Criar novo voo - GET
        /// </summary>
        [HttpGet]
        public IActionResult CreateVoo()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        /// <summary>
        /// Criar novo voo - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoo(Voo voo)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Voos.Add(voo);
                await _context.SaveChangesAsync();

                // Criar assentos automaticamente para o voo
                for (int i = 1; i <= voo.TotalLugares; i++)
                {
                    var assento = new Assento
                    {
                        VooId = voo.Id,
                        NumeroAssento = $"{(char)('A' + (i - 1) / 6)}{((i - 1) % 6) + 1}",
                        IsOccupied = false
                    };
                    _context.Assentos.Add(assento);
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Voo criado com sucesso!";
                return RedirectToAction(nameof(Voos));
            }

            return View(voo);
        }

        /// <summary>
        /// Editar voo - GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditVoo(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var voo = await _context.Voos.FindAsync(id);
            if (voo == null) return NotFound();

            return View(voo);
        }

        /// <summary>
        /// Editar voo - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoo(int id, Voo voo)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id != voo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voo);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Voo atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VooExists(voo.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Voos));
            }

            return View(voo);
        }

        /// <summary>
        /// Eliminar voo - GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DeleteVoo(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var voo = await _context.Voos.FirstOrDefaultAsync(m => m.Id == id);
            if (voo == null) return NotFound();

            return View(voo);
        }

        /// <summary>
        /// Eliminar voo - POST
        /// </summary>
        [HttpPost, ActionName("DeleteVoo")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVooConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var voo = await _context.Voos.FindAsync(id);
            if (voo != null)
            {
                _context.Voos.Remove(voo);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Voo eliminado com sucesso!";
            }

            return RedirectToAction(nameof(Voos));
        }

        // ==================== CRUD DE HOTÉIS ====================

        /// <summary>
        /// Lista de hotéis para gestão
        /// </summary>
        public async Task<IActionResult> Hoteis()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var hoteis = await _context.Hoteis.OrderBy(h => h.Nome).ToListAsync();
            return View(hoteis);
        }

        /// <summary>
        /// Criar novo hotel - GET
        /// </summary>
        [HttpGet]
        public IActionResult CreateHotel()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");
            return View();
        }

        /// <summary>
        /// Criar novo hotel - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHotel(Hotel hotel)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                _context.Hoteis.Add(hotel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hotel criado com sucesso!";
                return RedirectToAction(nameof(Hoteis));
            }

            return View(hotel);
        }

        /// <summary>
        /// Editar hotel - GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditHotel(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var hotel = await _context.Hoteis.FindAsync(id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        /// <summary>
        /// Editar hotel - POST
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHotel(int id, Hotel hotel)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id != hotel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hotel);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hotel atualizado com sucesso!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HotelExists(hotel.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Hoteis));
            }

            return View(hotel);
        }

        /// <summary>
        /// Eliminar hotel - GET
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> DeleteHotel(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            if (id == null) return NotFound();

            var hotel = await _context.Hoteis.FirstOrDefaultAsync(m => m.Id == id);
            if (hotel == null) return NotFound();

            return View(hotel);
        }

        /// <summary>
        /// Eliminar hotel - POST
        /// </summary>
        [HttpPost, ActionName("DeleteHotel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHotelConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var hotel = await _context.Hoteis.FindAsync(id);
            if (hotel != null)
            {
                _context.Hoteis.Remove(hotel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hotel eliminado com sucesso!";
            }

            return RedirectToAction(nameof(Hoteis));
        }

        // ==================== GESTÃO DE RESERVAS ====================

        /// <summary>
        /// Lista de todas as reservas
        /// </summary>
        public async Task<IActionResult> Reservas()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var reservas = await _context.Reservas
                .Include(r => r.User)
                .Include(r => r.Voo)
                .Include(r => r.Hotel)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            return View(reservas);
        }

        /// <summary>
        /// Lista de utilizadores
        /// </summary>
        public async Task<IActionResult> Utilizadores()
        {
            if (!IsAdmin()) return RedirectToAction("Index", "Home");

            var users = await _context.Users.OrderBy(u => u.NomeCompleto).ToListAsync();
            return View(users);
        }

        // Métodos auxiliares
        private bool VooExists(int id)
        {
            return _context.Voos.Any(e => e.Id == id);
        }

        private bool HotelExists(int id)
        {
            return _context.Hoteis.Any(e => e.Id == id);
        }
    }
}
