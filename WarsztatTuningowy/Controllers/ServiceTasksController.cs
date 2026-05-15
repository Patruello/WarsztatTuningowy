using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Controllers
{
    [Authorize(Roles = "Owner,Mechanic")]
    public class ServiceTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ServiceTasks
        public async Task<IActionResult> Index(int? orderId)
        {
            var query = _context.ServiceTasks
                .Include(st => st.Order)
                    .ThenInclude(o => o.Vehicle)
                .Include(st => st.AssignedEmployee)
                .AsQueryable();

            if (orderId.HasValue)
            {
                query = query.Where(st => st.OrderId == orderId.Value);

                ViewBag.OrderId = orderId.Value;
                ViewBag.OrderInfo = await _context.Orders
                    .Include(o => o.Vehicle)
                    .FirstOrDefaultAsync(o => o.Id == orderId.Value);
            }

            var tasks = await query
                .OrderBy(st => st.OrderId)
                .ThenBy(st => st.StartTime)
                .ToListAsync();

            return View(tasks);
        }

        // GET: ServiceTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceTask = await _context.ServiceTasks
                .Include(s => s.Order)
                    .ThenInclude(o => o.Vehicle)
                        .ThenInclude(v => v.Client)
                .Include(s => s.AssignedEmployee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (serviceTask == null)
            {
                return NotFound();
            }

            return View(serviceTask);
        }

        // GET: ServiceTasks/Create
        public IActionResult Create(int orderId)
        {
            ViewBag.OrderId = orderId;

            ViewData["AssignedEmployeeId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic)
                    .OrderBy(e => e.FullName),
                "Id", "FullName");
            return View();
        }

        // POST: ServiceTasks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Notes,OrderId,AssignedEmployeeId")] ServiceTask serviceTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(serviceTask);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Orders",
                    new { id = serviceTask.OrderId });
            }
            ViewBag.OrderId = serviceTask.OrderId;
            ViewData["AssignedEmployeeId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic),
                "Id", "FullName", serviceTask.AssignedEmployeeId);

            return View(serviceTask);
        }

        // GET: ServiceTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceTask = await _context.ServiceTasks
                .Include(st => st.AssignedEmployee)
                .FirstOrDefaultAsync(st => st.Id == id);
            if (serviceTask == null)
            {
                return NotFound();
            }

            ViewData["AssignedEmployeeId"] = new SelectList(_context.Employees.Where(e => e.Role == EmployeeRole.Mechanic), "Id", "FullName", serviceTask.AssignedEmployeeId);

            return View(serviceTask);
        }

        // POST: ServiceTasks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Notes,OrderId,AssignedEmployeeId")] ServiceTask serviceTask)
        {
            if (id != serviceTask.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingTask = await _context.ServiceTasks.FindAsync(id);
                if (existingTask == null) return NotFound();

                existingTask.Name = serviceTask.Name;
                existingTask.AssignedEmployeeId = serviceTask.AssignedEmployeeId;
                existingTask.Notes = serviceTask.Notes;

                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Orders", new { id = existingTask.OrderId });
            }

            ViewData["AssignedEmployeeId"] = new SelectList(_context.Employees.Where(e => e.Role == EmployeeRole.Mechanic), "Id", "FullName", serviceTask.AssignedEmployeeId);
            return View(serviceTask);
        }

        // GET: ServiceTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _context.ServiceTasks
                .Include(s => s.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: ServiceTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.ServiceTasks.FindAsync(id);
            if (task != null)
            {
                var orderId = task.OrderId;
                _context.ServiceTasks.Remove(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Orders",
                    new { id = orderId });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(int id, string? returnView = null)
        {
            var task = await _context.ServiceTasks.FindAsync(id);
            if (task == null) return NotFound();

            task.StartTimer();
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Timer uruchumiony dla {task.Name}";

            return returnView == "WorkView"
                ? RedirectToAction("WorkView", "Orders", new { id = task.OrderId })
                : RedirectToAction("Details", "Orders", new { id = task.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Stop(int id, string? returnView = null)
        {
            var task = await _context.ServiceTasks.FindAsync(id);
            if (task == null) return NotFound();

            task.StopTimer();
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Timer zatrzymany dla {task.Name}";

            return returnView == "WorkView"
                ? RedirectToAction("WorkView", "Orders", new { id = task.OrderId })
                : RedirectToAction("Details", "Orders", new { id = task.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pause(int id, int pauseMinutes, string? returnView = null)
        {
            var task = await _context.ServiceTasks.FindAsync(id);
            if (task == null) return NotFound();

            task.PauseTimer(pauseMinutes);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Zarejestrowano przerwę: {pauseMinutes} min";

            return returnView == "WorkView"
                ? RedirectToAction("WorkView", "Orders", new { id = task.OrderId })
                : RedirectToAction("Details", "Orders", new { id = task.OrderId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNotes(int id, string notes)
        {
            var task = await _context.ServiceTasks.FindAsync(id);
            if (task == null) return NotFound();

            task.AddNote(notes);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Notatka dodana";

            return RedirectToOrderView(task.OrderId);
        }

        private IActionResult RedirectToOrderView(int orderId)
        {
            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("WorkView"))
                return RedirectToAction("WorkView", "Orders",
                    new { id = orderId });

            return RedirectToAction("Details", "Orders",
                new { id = orderId });
        }
    }
}
