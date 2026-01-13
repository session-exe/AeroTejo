using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using AeroTejo.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    /// <summary>
    /// Controller para gestão de reservas
    /// </summary>
    public class ReservaController : Controller
    {
        private readonly AeroTejoContext _context;

        public ReservaController(AeroTejoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Configuração da reserva: seleção de assento e dados de passageiros
        /// Requer login
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Configure()
        {
            // Verificar autenticação
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Precisa de fazer login para continuar.";
                return RedirectToAction("Login", "Account");
            }

            // Verificar se há um voo selecionado
            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            if (!vooId.HasValue)
            {
                TempData["ErrorMessage"] = "Selecione um voo primeiro.";
                return RedirectToAction("Index", "Voo");
            }

            // Obter o voo e os assentos disponíveis
            var voo = await _context.Voos
                .Include(v => v.Assentos)
                .FirstOrDefaultAsync(v => v.Id == vooId.Value);

            if (voo == null)
            {
                TempData["ErrorMessage"] = "Voo não encontrado.";
                return RedirectToAction("Index", "Voo");
            }

            // Obter hotel se selecionado
            var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
            Hotel? hotel = null;
            if (hotelId.HasValue)
            {
                hotel = await _context.Hoteis.FindAsync(hotelId.Value);
            }

            ViewBag.Voo = voo;
            ViewBag.Hotel = hotel;
            ViewBag.AssentosDisponiveis = voo.Assentos.Where(a => !a.IsOccupied).ToList();

            return View();
        }

        /// <summary>
        /// Processar a configuração e avançar para checkout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configure(ReservaConfigViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Validar datas
                if (model.DataCheckOut <= model.DataCheckIn)
                {
                    ModelState.AddModelError("DataCheckOut", "A data de check-out deve ser posterior à data de check-in.");
                    return await ReloadConfigureView(model);
                }

                // Verificar se o assento ainda está disponível
                var assento = await _context.Assentos.FindAsync(model.AssentoId);
                if (assento == null || assento.IsOccupied)
                {
                    ModelState.AddModelError("AssentoId", "Assento não disponível.");
                    return await ReloadConfigureView(model);
                }

                // Guardar dados na sessão
                HttpContext.Session.SetInt32("SelectedAssentoId", model.AssentoId);
                HttpContext.Session.SetString("DataCheckIn", model.DataCheckIn.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString("DataCheckOut", model.DataCheckOut.ToString("yyyy-MM-dd"));
                HttpContext.Session.SetString("DadosPassageiros", model.DadosPassageiros);

                // Avançar para checkout
                return RedirectToAction(nameof(Checkout));
            }

            return await ReloadConfigureView(model);
        }

        /// <summary>
        /// Página de checkout com dados de pagamento e faturação
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            // Verificar autenticação
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Verificar se há dados de reserva na sessão
            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            var assentoId = HttpContext.Session.GetInt32("SelectedAssentoId");

            if (!vooId.HasValue || !assentoId.HasValue)
            {
                TempData["ErrorMessage"] = "Dados de reserva incompletos.";
                return RedirectToAction("Index", "Voo");
            }

            // Calcular valor total
            var voo = await _context.Voos.FindAsync(vooId.Value);
            var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
            Hotel? hotel = null;
            decimal valorTotal = voo!.Preco;

            if (hotelId.HasValue)
            {
                hotel = await _context.Hoteis.FindAsync(hotelId.Value);
                if (hotel != null)
                {
                    var checkIn = DateTime.Parse(HttpContext.Session.GetString("DataCheckIn")!);
                    var checkOut = DateTime.Parse(HttpContext.Session.GetString("DataCheckOut")!);
                    int noites = (checkOut - checkIn).Days;
                    valorTotal += hotel.PrecoPorNoite * noites;
                }
            }

            ViewBag.Voo = voo;
            ViewBag.Hotel = hotel;
            ViewBag.ValorTotal = valorTotal;

            return View();
        }

        /// <summary>
        /// Processar o checkout e finalizar a reserva
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                var vooId = HttpContext.Session.GetInt32("SelectedVooId");
                var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
                var assentoId = HttpContext.Session.GetInt32("SelectedAssentoId");

                if (!userId.HasValue || !vooId.HasValue || !assentoId.HasValue)
                {
                    TempData["ErrorMessage"] = "Erro ao processar reserva.";
                    return RedirectToAction("Index", "Home");
                }

                // Obter dados da sessão
                var checkIn = DateTime.Parse(HttpContext.Session.GetString("DataCheckIn")!);
                var checkOut = DateTime.Parse(HttpContext.Session.GetString("DataCheckOut")!);
                var dadosPassageiros = HttpContext.Session.GetString("DadosPassageiros")!;

                // Calcular valor total
                var voo = await _context.Voos.FindAsync(vooId.Value);
                decimal valorTotal = voo!.Preco;

                if (hotelId.HasValue)
                {
                    var hotel = await _context.Hoteis.FindAsync(hotelId.Value);
                    if (hotel != null)
                    {
                        int noites = (checkOut - checkIn).Days;
                        valorTotal += hotel.PrecoPorNoite * noites;
                    }
                }

                // Obter o assento e marcar como ocupado
                var assento = await _context.Assentos.FindAsync(assentoId.Value);
                assento!.IsOccupied = true;

                // Criar a reserva
                var reserva = new Reserva
                {
                    UserId = userId.Value,
                    VooId = vooId.Value,
                    HotelId = hotelId,
                    DataCheckIn = checkIn,
                    DataCheckOut = checkOut,
                    ValorTotal = valorTotal,
                    DataReserva = DateTime.Now,
                    NumeroAssento = assento.NumeroAssento,
                    DadosPassageiros = dadosPassageiros
                };

                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                // Criar dados de faturação
                var faturacao = new Faturacao
                {
                    ReservaId = reserva.Id,
                    NIF = model.NIF,
                    Morada = model.Morada,
                    Nome = model.Nome,
                    DataEmissao = DateTime.Now
                };

                _context.Faturacoes.Add(faturacao);
                await _context.SaveChangesAsync();

                // Limpar dados da sessão
                HttpContext.Session.Remove("SelectedVooId");
                HttpContext.Session.Remove("SelectedHotelId");
                HttpContext.Session.Remove("SelectedAssentoId");
                HttpContext.Session.Remove("DataCheckIn");
                HttpContext.Session.Remove("DataCheckOut");
                HttpContext.Session.Remove("DadosPassageiros");

                TempData["SuccessMessage"] = "Reserva efetuada com sucesso!";
                return RedirectToAction(nameof(Confirmation), new { id = reserva.Id });
            }

            return await ReloadCheckoutView(model);
        }

        /// <summary>
        /// Página de confirmação com geração de PDF
        /// </summary>
        public async Task<IActionResult> Confirmation(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.User)
                .Include(r => r.Voo)
                .Include(r => r.Hotel)
                .Include(r => r.Faturacao)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
            {
                return NotFound();
            }

            // Verificar se o utilizador tem permissão para ver esta reserva
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userId != reserva.UserId && userRole != "Administrador")
            {
                return Forbid();
            }

            return View(reserva);
        }

        /// <summary>
        /// Minhas reservas (para passageiros)
        /// </summary>
        public async Task<IActionResult> MinhasReservas()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var reservas = await _context.Reservas
                .Include(r => r.Voo)
                .Include(r => r.Hotel)
                .Where(r => r.UserId == userId.Value)
                .OrderByDescending(r => r.DataReserva)
                .ToListAsync();

            return View(reservas);
        }

        // Métodos auxiliares privados
        private async Task<IActionResult> ReloadConfigureView(ReservaConfigViewModel model)
        {
            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            var voo = await _context.Voos.Include(v => v.Assentos).FirstOrDefaultAsync(v => v.Id == vooId);
            var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
            Hotel? hotel = null;
            if (hotelId.HasValue)
            {
                hotel = await _context.Hoteis.FindAsync(hotelId.Value);
            }

            ViewBag.Voo = voo;
            ViewBag.Hotel = hotel;
            ViewBag.AssentosDisponiveis = voo!.Assentos.Where(a => !a.IsOccupied).ToList();

            return View(model);
        }

        private async Task<IActionResult> ReloadCheckoutView(CheckoutViewModel model)
        {
            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            var voo = await _context.Voos.FindAsync(vooId);
            var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
            Hotel? hotel = null;
            decimal valorTotal = voo!.Preco;

            if (hotelId.HasValue)
            {
                hotel = await _context.Hoteis.FindAsync(hotelId.Value);
                if (hotel != null)
                {
                    var checkIn = DateTime.Parse(HttpContext.Session.GetString("DataCheckIn")!);
                    var checkOut = DateTime.Parse(HttpContext.Session.GetString("DataCheckOut")!);
                    int noites = (checkOut - checkIn).Days;
                    valorTotal += hotel.PrecoPorNoite * noites;
                }
            }

            ViewBag.Voo = voo;
            ViewBag.Hotel = hotel;
            ViewBag.ValorTotal = valorTotal;

            return View(model);
        }
    }
}
