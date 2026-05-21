using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Extensions;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Controllers
{
    [Authorize(Roles = "Owner")]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.ServiceTasks)
                .Include(e => e.AssignedOrders)
                .OrderBy(e => e.Role)
                .ThenBy(e => e.FullName)
                .ToListAsync();

            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.ServiceTasks)
                    .ThenInclude(st => st.Order)
                        .ThenInclude(o => o.Vehicle)
                .Include(e => e.AssignedOrders)
                    .ThenInclude(o => o.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["Role"] = new SelectList(Enum.GetValues<EmployeeRole>().Select(r => new { Value = r, Text = r.GetDisplayName() }), "Value", "Text");

            var assignedUserIds = _context.Employees
                .Where(e => e.UserId != null)
                .Select(e => e.UserId)
                .ToList();

            ViewData["UserID"] = new SelectList(
                _userManager.Users
                    .Where(u => !assignedUserIds.Contains(u.Id))
                    .OrderBy(u => u.Email)
                    .Select(u => new { u.Id, u.Email }),
                "Id", "Email");

            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName,Role,HourlyRateInternal,HourlyRateClient,UserId")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = employee.Id });
            }

            ViewData["Role"] = new SelectList(Enum.GetValues<EmployeeRole>().Select(r => new { Value = r, Text = r.GetDisplayName() }), "Value", "Text", employee.Role);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["Role"] = new SelectList(Enum.GetValues<EmployeeRole>().Select(r => new { Value = r, Text = r.GetDisplayName() }), "Value", "Text", employee.Role);

            var assignedUserIds = _context.Employees
                .Where(e => e.UserId != null && e.Id != id)
                .Select(e => e.UserId)
                .ToList();

            ViewData["UserID"] = new SelectList(
                _userManager.Users
                    .Where(u => !assignedUserIds.Contains(u.Id))
                    .OrderBy(u => u.Email)
                    .Select(u => new { u.Id, u.Email }),
                "Id", "Email", employee.UserId);

            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Role,HourlyRateInternal,HourlyRateClient,UserId")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existing = await _context.Employees.FindAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.FullName = employee.FullName;
                existing.Role = employee.Role;
                existing.HourlyRateInternal = employee.HourlyRateInternal;
                existing.HourlyRateClient = employee.HourlyRateClient;
                existing.UserId = employee.UserId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = employee.Id });
            }

            ViewData["Role"] = new SelectList(Enum.GetValues<EmployeeRole>().Select(r => new { Value = r, Text = r.GetDisplayName() }), "Value", "Text", employee.Role);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.AssignedOrders)
                .Include(e => e.ServiceTasks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (employee == null)
            {
                return NotFound();
            }
            if (employee.AssignedOrders.Any(o => o.Status != OrderStatus.Completed))
            {
                TempData["Error"] = "Nie można usunąć pracownika, który ma aktywne zlecenia.";
                return RedirectToAction(nameof(Details), new { id = employee.Id });
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
