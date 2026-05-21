using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Models.Domain;

namespace WarsztatTuningowy.Controllers
{
    [Authorize(Roles = "Owner,Storekeeper")]
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parts
        public async Task<IActionResult> Index()
        {
            var parts = await _context.Parts
                .Include(p => p.OrderParts)
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(parts);
        }

        // GET: Parts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .Include(p => p.OrderParts)
                    .ThenInclude(op => op.Order)
                        .ThenInclude(o => o.Vehicle)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // GET: Parts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Parts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,WholesalePrice,RetailPrice,Stock,MinStock,SupplierName,IsStockPart")] Part part)
        {
            if (ModelState.IsValid)
            {
                _context.Add(part);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(part);
        }

        // GET: Parts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }
            return View(part);
        }

        // POST: Parts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,WholesalePrice,RetailPrice,MinStock,SupplierName,IsStockPart")] Part part)
        {
            if (id != part.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.Parts.FindAsync(id);
                if (existing == null) return NotFound();

                existing.Name = part.Name;
                existing.WholesalePrice = part.WholesalePrice;
                existing.RetailPrice = part.RetailPrice;
                existing.MinStock = part.MinStock;
                existing.SupplierName = part.SupplierName;
                existing.IsStockPart = part.IsStockPart;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(part);
        }

        // GET: Parts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var part = await _context.Parts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part != null)
            {
                _context.Parts.Remove(part);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Parts/StockAdjustment/5
        public async Task<IActionResult> StockAdjustment(int? id)
        {
            if (id == null) return NotFound();

            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            return View(part);
        }

        // POST: /Parts/StockAdjustment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockAdjustment(int id, int quantity, string reason)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null)
            {
                return NotFound();
            }

            if (!part.AdjustStock(quantity))
            {
                TempData["Error"] =
                    $"Nie można wydać {Math.Abs(quantity)} szt. " +
                    $"Dostępnych jest tylko {part.Stock} szt.";
                return RedirectToAction(nameof(Details), new { id });
            }
            await _context.SaveChangesAsync();

            TempData["Success"] = quantity > 0
                ? $"Dodano {quantity} szt. do magazynu. Nowy stan: {part.Stock} szt."
                : $"Wydano {Math.Abs(quantity)} szt. z magazynu. Nowy stan: {part.Stock} szt.";

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
