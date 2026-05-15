using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Models.Domain;

namespace WarsztatTuningowy.Controllers
{
    [Authorize(Roles = "Owner")]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles.Include(v => v.Client).OrderBy(v => v.Brand).ThenBy(v => v.Model);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Client)
                .Include(v => v.Orders)
                    .ThenInclude(o => o.ServiceTasks)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Create(int? clientId = null, string? returnUrl = null)
        {
            ViewData["ClientId"] = new SelectList(_context.Clients.OrderBy(c => c.FullName), "Id", "FullName", clientId);
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        // POST: Vehicles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VIN,Brand,Model,Year,EngineType,ECUType,ClientId")] Vehicle vehicle, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction(nameof(Details), new { id = vehicle.Id });
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewData["ClientId"] = new SelectList(_context.Clients.OrderBy(c => c.FullName), "Id", "FullName", vehicle.ClientId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients.OrderBy(c => c.FullName), "Id", "FullName", vehicle.ClientId);
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VIN,Brand,Model,Year,EngineType,ECUType,ClientId")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                var existingVehicle = await _context.Vehicles.FindAsync(id);
                if (existingVehicle == null)
                {
                    return NotFound();
                }

                existingVehicle.VIN = vehicle.VIN;
                existingVehicle.Brand = vehicle.Brand;
                existingVehicle.Model = vehicle.Model;
                existingVehicle.Year = vehicle.Year;
                existingVehicle.EngineType = vehicle.EngineType;
                existingVehicle.ECUType = vehicle.ECUType;
                existingVehicle.ClientId = vehicle.ClientId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = existingVehicle.Id });
            }

            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "FullName", vehicle.ClientId);
            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Client)
                .Include(v => v.Orders)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
