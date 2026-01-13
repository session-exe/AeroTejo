using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using AeroTejo.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AeroTejo.Controllers
{
    public class ReservaController : Controller
    {
        private readonly AeroTejoContext _context;

        public ReservaController(AeroTejoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Configure()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var vooId = HttpContext.Session.GetInt32("SelectedVooId");
            if (!vooId.HasValue) return RedirectToAction("Index", "Home");

            var voo = await _context.Voos.Include(v => v.Assentos).FirstOrDefaultAsync(v => v.Id == vooId.Value);
            if (voo == null) return NotFound();

            var hotelId = HttpContext.Session.GetInt32("SelectedHotelId");
            Hotel? hotel = null;
            if (hotelId.HasValue) hotel = await _context.Hoteis.FindAsync(hotelId.Value);

            var viewModel = new ReservaConfigViewModel
            {
                VooId = voo.Id,
                Voo = voo,
                HotelId = hotelId,
                Hotel = hotel,
                DataCheckIn = voo.DataHora.Date,
                DataCheckOut = voo.DataHora.Date.AddDays(1),
                // CORREÇÃO AQUI: Ordenar por ID antes de selecionar o texto
                AssentosDisponiveis = voo.Assentos
                    .Where(a => !a.IsOccupied)
                    .OrderBy(a => a.Id)
                    .Select(a => a.NumeroAssento)
                    .ToList(),
                Passageiros = new List<PassageiroInfo> { new PassageiroInfo() }
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Configure(ReservaConfigViewModel model)
        {
            var voo = await _context.Voos.FindAsync(model.VooId);
            if (!ModelState.IsValid) return await ReloadConfigureView(model);

            var assentosEscolhidos = model.Passageiros.Select(p => p.AssentoSelecionado).ToList();
            if (assentosEscolhidos.Count != assentosEscolhidos.Distinct().Count())
            {
                ModelState.AddModelError("", "Assentos duplicados."); return await ReloadConfigureView(model);
            }

            if (model.HotelId.HasValue)
            {
                if (model.DataCheckIn.HasValue && model.DataCheckIn.Value.Date < voo!.DataHora.Date)
                {
                    ModelState.AddModelError("DataCheckIn", "Check-in inválido."); return await ReloadConfigureView(model);
                }
                if (model.DataCheckOut <= model.DataCheckIn)
                {
                    ModelState.AddModelError("DataCheckOut", "Check-out inválido."); return await ReloadConfigureView(model);
                }
            }

            var passageirosJson = JsonSerializer.Serialize(model.Passageiros);
            HttpContext.Session.SetString("Reserva_Passageiros", passageirosJson);
            HttpContext.Session.SetInt32("Reserva_VooId", model.VooId);

            if (model.HotelId.HasValue)
            {
                HttpContext.Session.SetInt32("Reserva_HotelId", model.HotelId.Value);
                HttpContext.Session.SetString("Reserva_CheckIn", model.DataCheckIn?.ToString("yyyy-MM-dd") ?? "");
                HttpContext.Session.SetString("Reserva_CheckOut", model.DataCheckOut?.ToString("yyyy-MM-dd") ?? "");
            }
            else { HttpContext.Session.Remove("Reserva_HotelId"); }

            return RedirectToAction(nameof(Checkout));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var vooId = HttpContext.Session.GetInt32("Reserva_VooId");
            if (!vooId.HasValue) return RedirectToAction("Index", "Home");

            var voo = await _context.Voos.FindAsync(vooId.Value);
            var hotelId = HttpContext.Session.GetInt32("Reserva_HotelId");
            var passageiros = JsonSerializer.Deserialize<List<PassageiroInfo>>(HttpContext.Session.GetString("Reserva_Passageiros")!) ?? new();

            decimal total = voo!.Preco * passageiros.Count;
            Hotel? hotel = null;
            int noites = 0;

            if (hotelId.HasValue)
            {
                hotel = await _context.Hoteis.FindAsync(hotelId.Value);
                if (hotel != null)
                {
                    DateTime.TryParse(HttpContext.Session.GetString("Reserva_CheckIn"), out DateTime inD);
                    DateTime.TryParse(HttpContext.Session.GetString("Reserva_CheckOut"), out DateTime outD);
                    noites = (outD - inD).Days; if (noites < 1) noites = 1;
                    total += hotel.PrecoPorNoite * noites;
                }
            }

            ViewBag.Voo = voo; ViewBag.Hotel = hotel; ViewBag.ValorTotal = total;
            ViewBag.TotalPassageiros = passageiros.Count; ViewBag.NumNoites = noites;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid) return await Checkout();

            var userId = HttpContext.Session.GetInt32("UserId");
            var vooId = HttpContext.Session.GetInt32("Reserva_VooId");
            var hotelId = HttpContext.Session.GetInt32("Reserva_HotelId");
            var passageiros = JsonSerializer.Deserialize<List<PassageiroInfo>>(HttpContext.Session.GetString("Reserva_Passageiros")!) ?? new();

            // Ocupar Assentos
            foreach (var p in passageiros)
            {
                var assento = await _context.Assentos.FirstOrDefaultAsync(a => a.VooId == vooId && a.NumeroAssento == p.AssentoSelecionado);
                if (assento != null) assento.IsOccupied = true;
            }

            // Calcular Total e Datas
            var voo = await _context.Voos.FindAsync(vooId);
            decimal total = voo!.Preco * passageiros.Count;
            DateTime inD = voo.DataHora.Date, outD = voo.DataHora.Date;

            if (hotelId.HasValue)
            {
                var hotel = await _context.Hoteis.FindAsync(hotelId);
                DateTime.TryParse(HttpContext.Session.GetString("Reserva_CheckIn"), out inD);
                DateTime.TryParse(HttpContext.Session.GetString("Reserva_CheckOut"), out outD);
                int dias = (outD - inD).Days; if (dias < 1) dias = 1;
                total += hotel!.PrecoPorNoite * dias;
            }

            var reserva = new Reserva
            {
                UserId = userId.Value,
                VooId = vooId.Value,
                HotelId = hotelId,
                DataCheckIn = inD,
                DataCheckOut = outD,
                ValorTotal = total,
                DataReserva = DateTime.Now,
                NumeroAssento = "Múltiplos",
                DadosPassageiros = JsonSerializer.Serialize(passageiros)
            };
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            _context.Faturacoes.Add(new Faturacao
            {
                ReservaId = reserva.Id,
                NIF = model.NIF ?? "N/A",
                Morada = model.Morada,
                Nome = model.Nome,
                DataEmissao = DateTime.Now
            });
            await _context.SaveChangesAsync();

            // Limpar Sessão
            HttpContext.Session.Remove("Reserva_VooId"); HttpContext.Session.Remove("Reserva_HotelId");
            HttpContext.Session.Remove("Reserva_Passageiros"); HttpContext.Session.Remove("SelectedVooId");
            HttpContext.Session.Remove("SelectedHotelId"); HttpContext.Session.Remove("Reserva_CheckIn");
            HttpContext.Session.Remove("Reserva_CheckOut");

            return RedirectToAction(nameof(Confirmation), new { id = reserva.Id });
        }

        // ==================== NOVO: CANCELAR RESERVA (Passageiro) ====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var reserva = await _context.Reservas.FindAsync(id);

            // Só pode cancelar a própria reserva
            if (reserva == null || reserva.UserId != userId.Value) return NotFound();

            // Libertar Assentos
            if (!string.IsNullOrEmpty(reserva.DadosPassageiros))
            {
                var passageiros = JsonSerializer.Deserialize<List<PassageiroInfo>>(reserva.DadosPassageiros);
                if (passageiros != null)
                {
                    foreach (var p in passageiros)
                    {
                        var assento = await _context.Assentos
                            .FirstOrDefaultAsync(a => a.VooId == reserva.VooId && a.NumeroAssento == p.AssentoSelecionado);
                        if (assento != null) assento.IsOccupied = false;
                    }
                }
            }

            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Reserva cancelada com sucesso.";
            return RedirectToAction(nameof(MinhasReservas));
        }

        // ==================== PDF e Auxiliares ====================
        public async Task<IActionResult> DownloadFatura(int id)
        {
            var reserva = await _context.Reservas.Include(r => r.Voo).Include(r => r.Hotel).Include(r => r.User).Include(r => r.Faturacao).FirstOrDefaultAsync(r => r.Id == id);
            if (reserva == null) return NotFound();

            // Permitir download se for Admin OU o dono da reserva
            var userId = HttpContext.Session.GetInt32("UserId");
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Administrador" && reserva.UserId != userId) return Forbid();

            var passageiros = JsonSerializer.Deserialize<List<PassageiroInfo>>(reserva.DadosPassageiros ?? "[]");
            int qtd = passageiros?.Count ?? 1;

            var document = Document.Create(container => {
                container.Page(page => {
                    page.Size(PageSizes.A4); page.Margin(2, Unit.Centimetre); page.PageColor(Colors.White); page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));
                    page.Header().Row(row => {
                        row.RelativeItem().Column(col => { col.Item().Text("AeroTejo").SemiBold().FontSize(24).FontColor(Colors.DeepPurple.Medium); col.Item().Text("Lisboa, Portugal"); });
                        row.ConstantItem(150).Column(col => { col.Item().Text($"Fatura #{reserva.Id}").SemiBold(); col.Item().Text($"{reserva.DataReserva:d}"); });
                    });
                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col => {
                        col.Item().Text($"Cliente: {reserva.Faturacao?.Nome ?? "N/A"} | NIF: {reserva.Faturacao?.NIF ?? "N/A"}");
                        col.Item().PaddingVertical(10).Table(table => {
                            table.ColumnsDefinition(c => { c.RelativeColumn(3); c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
                            table.Header(h => { h.Cell().Text("Desc"); h.Cell().Text("Qtd"); h.Cell().Text("Preço"); h.Cell().Text("Total"); });

                            table.Cell().Text($"Voo: {reserva.Voo?.Origem}-{reserva.Voo?.Destino}");
                            table.Cell().Text($"{qtd}"); table.Cell().Text($"{reserva.Voo?.Preco:C}"); table.Cell().Text($"{(reserva.Voo?.Preco * qtd):C}");

                            if (reserva.Hotel != null)
                            {
                                int dias = (reserva.DataCheckOut - reserva.DataCheckIn).Days; if (dias < 1) dias = 1;
                                table.Cell().Text($"Hotel: {reserva.Hotel.Nome}"); table.Cell().Text($"{dias} noites");
                                table.Cell().Text($"{reserva.Hotel.PrecoPorNoite:C}"); table.Cell().Text($"{(reserva.Hotel.PrecoPorNoite * dias):C}");
                            }
                        });
                        col.Item().AlignRight().Text($"Total: {reserva.ValorTotal:C}").Bold().FontSize(14);
                    });
                });
            });
            return File(document.GeneratePdf(), "application/pdf", $"Fatura_{id}.pdf");
        }

        private async Task<IActionResult> ReloadConfigureView(ReservaConfigViewModel model)
        {
            model.Voo = await _context.Voos.FindAsync(model.VooId);
            if (model.HotelId.HasValue) model.Hotel = await _context.Hoteis.FindAsync(model.HotelId);

            // CORREÇÃO AQUI TAMBÉM: Ordenar por ID antes de selecionar o texto
            model.AssentosDisponiveis = await _context.Assentos
                .Where(a => a.VooId == model.VooId && !a.IsOccupied)
                .OrderBy(a => a.Id)
                .Select(a => a.NumeroAssento)
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var r = await _context.Reservas.Include(x => x.User).Include(x => x.Voo).Include(x => x.Hotel).Include(x => x.Faturacao).FirstOrDefaultAsync(x => x.Id == id);
            return r == null ? NotFound() : View(r);
        }

        public async Task<IActionResult> MinhasReservas()
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            if (!uid.HasValue) return RedirectToAction("Login", "Account");
            return View(await _context.Reservas.Include(r => r.Voo).Include(r => r.Hotel).Where(r => r.UserId == uid.Value).OrderByDescending(r => r.DataReserva).ToListAsync());
        }
    }
}