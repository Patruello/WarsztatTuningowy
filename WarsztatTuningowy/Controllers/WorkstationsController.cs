using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Extensions;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Controllers
{
    [Authorize]
    public class WorkstationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkstationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Workstations
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Index()
        {
            var workstations = await _context.Workstations
                .Include(w => w.Assignments)
                    .ThenInclude(a => a.Order)
                        .ThenInclude(o => o.Vehicle)
                .OrderBy(w => w.Type)
                .ThenBy(w => w.Name)
                .ToListAsync();

            return View(workstations);
        }

        // GET: Workstations/Details/5
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstation = await _context.Workstations
                .Include(w => w.Assignments)
                    .ThenInclude(a => a.Order)
                        .ThenInclude(o => o.Vehicle)
                            .ThenInclude(v => v.Client)
                .FirstOrDefaultAsync(w => w.Id == id);
            if (workstation == null)
            {
                return NotFound();
            }

            return View(workstation);
        }

        // GET: Workstations/Create
        [Authorize(Roles = "Owner")]
        public IActionResult Create()
        {
            ViewData["Type"] = new SelectList(
                Enum.GetValues<WorkstationType>()
                    .Select(t => new
                    {
                        Value = t,
                        Text = t.GetDisplayName()
                    }),
                "Value",
                "Text");

            return View();
        }

        // POST: Workstations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create([Bind("Name,Type")] Workstation workstation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workstation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Type"] = new SelectList(
                Enum.GetValues<WorkstationType>()
                    .Select(t => new
                    {
                        Value = t,
                        Text = t.GetDisplayName()
                    }),
                "Value",
                "Text");

            return View(workstation);
        }

        // GET: Workstations/Edit/5
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstation = await _context.Workstations.FindAsync(id);
            if (workstation == null)
            {
                return NotFound();
            }

            ViewData["Type"] = new SelectList(
                Enum.GetValues<WorkstationType>()
                    .Select(t => new
                    {
                        Value = t,
                        Text = t.GetDisplayName()
                    }),
                "Value",
                "Text",
                workstation.Type);

            return View(workstation);
        }

        // POST: Workstations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type")] Workstation workstation)
        {
            if (id != workstation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingWorkstation = await _context.Workstations.FindAsync(id);
                if (existingWorkstation == null)
                {
                    return NotFound();
                }

                existingWorkstation.Name = workstation.Name;
                existingWorkstation.Type = workstation.Type;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewData["Type"] = new SelectList(
                Enum.GetValues<WorkstationType>()
                    .Select(t => new
                    {
                        Value = t,
                        Text = t.GetDisplayName()
                    }),
                "Value",
                "Text");

            return View(workstation);
        }

        // GET: Workstations/Delete/5
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workstation = await _context.Workstations
                .Include(w => w.Assignments)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (workstation == null)
            {
                return NotFound();
            }
            if (workstation.IsOccupied)
            {
                TempData["Error"] = "Nie można usunąć stanowiska, które jest aktualnie zajęte.";
                return RedirectToAction(nameof(Details), new { id });
            }

            return View(workstation);
        }

        // POST: Workstations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workstation = await _context.Workstations.FindAsync(id);
            if (workstation != null)
            {
                _context.Workstations.Remove(workstation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: /Workstations/Release/5
        [HttpPost]
        [Authorize(Roles = "Owner,Mechanic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Release(int id, int orderId)
        {
            var assignment = await _context.WorkstationAssignments
                .Include(wa => wa.Workstation)
                .FirstOrDefaultAsync(wa =>
                    wa.WorkstationId == id &&
                    wa.OrderId == orderId &&
                    wa.ReleasedAt == null);

            if (assignment == null)
            {
                return NotFound();
            }

            if(assignment.Workstation == null)
            {
                TempData["Error"] = "Nie można zwolnić stanowiska";
                return RedirectToAction("WorkView", "Orders", new { id = orderId });
            }

            assignment.Release();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Stanowisko zwolnione.";
            return RedirectToAction("WorkView", "Orders", new { id = orderId });
        }

        // POST: /Workstations/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Mechanic")]
        public async Task<IActionResult> Assign(int workstationId, int orderId)
        {
            var workstation = await _context.Workstations.FindAsync(workstationId);

            if (workstation == null || workstation.IsOccupied)
            {
                TempData["Error"] = "Stanowisko jest już zajęte.";
                return RedirectToAction("WorkView", "Orders", new { id = orderId });
            }

            var assignment = new WorkstationAssignment
            {
                WorkstationId = workstationId,
                OrderId = orderId,
                AssignedAt = DateTime.Now
            };

            workstation.Occupy();
            _context.WorkstationAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Przypisano stanowisko {workstation.Name}.";
            return RedirectToAction("WorkView", "Orders", new { id = orderId });
        }
    }
}
