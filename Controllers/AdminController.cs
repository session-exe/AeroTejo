using Microsoft.AspNetCore.Mvc;
using AeroTejo.Data;
using AeroTejo.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroTejo.Controllers
{
    public class AdminController : Controller
    {
        private readonly AeroTejoContext _context;

        public AdminController(AeroTejoContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        // ==================== GESTÃO DE VOOS ====================
        public async Task<IActionResult> Voos() => View(await _context.Voos.ToListAsync());

        [HttpGet]
        public IActionResult CreateVoo() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoo(Voo voo)
        {
            if (ModelState.IsValid)
            {
                var letras = new[] { "A", "B", "C", "D", "E", "F" };
                int filas = voo.TotalLugares / 6;
                voo.Assentos = new List<Assento>();

                for (int f = 1; f <= filas; f++)
                {
                    foreach (var l in letras)
                    {
                        voo.Assentos.Add(new Assento { NumeroAssento = $"{f}{l}", IsOccupied = false });
                    }
                }
                _context.Voos.Add(voo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Voos));
            }
            return View(voo);
        }

        [HttpGet]
        public async Task<IActionResult> EditVoo(int id)
        {
            var voo = await _context.Voos.FindAsync(id);
            if (voo == null) return NotFound();
            return View(voo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoo(int id, Voo voo)
        {
            if (id != voo.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(voo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Voos));
            }
            return View(voo);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteVoo(int id)
        {
            var voo = await _context.Voos.FindAsync(id);
            if (voo != null) { _context.Voos.Remove(voo); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Voos));
        }

        // ==================== GESTÃO DE HOTÉIS ====================
        public async Task<IActionResult> Hoteis() => View(await _context.Hoteis.ToListAsync());

        [HttpGet]
        public IActionResult CreateHotel() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHotel(Hotel hotel)
        {
            if (ModelState.IsValid)
            {
                _context.Hoteis.Add(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Hoteis));
            }
            return View(hotel);
        }

        [HttpGet]
        public async Task<IActionResult> EditHotel(int id)
        {
            var hotel = await _context.Hoteis.FindAsync(id);
            if (hotel == null) return NotFound();
            return View(hotel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHotel(int id, Hotel hotel)
        {
            if (id != hotel.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(hotel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Hoteis));
            }
            return View(hotel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var hotel = await _context.Hoteis.FindAsync(id);
            if (hotel != null) { _context.Hoteis.Remove(hotel); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Hoteis));
        }
    }
}