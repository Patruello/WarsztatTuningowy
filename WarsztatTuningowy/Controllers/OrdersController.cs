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
using WarsztatTuningowy.Models.Enums;
using WarsztatTuningowy.Services;

namespace WarsztatTuningowy.Controllers
{
    [Authorize(Roles = "Owner,Mechanic")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfService _pdfService;

        public OrdersController(ApplicationDbContext context, PdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.Vehicle).ThenInclude(v => v.Client).Include(o => o.DefaultMechanic).OrderByDescending(o => o.CreatedAt);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.DefaultMechanic)
                .Include(o => o.ServiceTasks)
                    .ThenInclude(st => st.AssignedEmployee)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .Include(o => o.WorkstationAssignments)
                    .ThenInclude(wa => wa.Workstation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles
                    .Include(v => v.Client)
                    .OrderBy(v => v.Brand)
                    .Select(v => new {v.Id, Display = $"{v.Client.FullName} - {v.Brand} {v.Model} ({v.Year})"}),
                "Id",
                "Display");

            ViewData["DefaultMechanicId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic)
                    .OrderBy(e => e.FullName),
                "Id",
                "FullName");
            
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleId,DefaultMechanicId,EstimatedHours,DepositPaid")] Order order, [FromForm] List<int> tuningGoals)
        {
            order.TuningGoal = TuningGoal.None;
            foreach (var goal in tuningGoals)
            {
                order.TuningGoal |= (TuningGoal)goal;
            }

            if (!Order.IsValidTuningGoal(order.TuningGoal))
                ModelState.AddModelError("TuningGoal", "Można wybrać tylko jeden Stage (1, 2 lub 3).");

            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles
                    .Include(v => v.Client)
                    .OrderBy(v => v.Brand)
                    .Select(v => new { v.Id, Display = $"{v.Client.FullName} - {v.Brand} {v.Model} ({v.Year})" }),
                "Id",
                "Display");

            ViewData["DefaultMechanicId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic)
                    .OrderBy(e => e.FullName),
                "Id",
                "FullName");

            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles
                    .Include(v => v.Client)
                    .OrderBy(v => v.Brand)
                    .Select(v => new { v.Id, Display = $"{v.Client.FullName} - {v.Brand} {v.Model} ({v.Year})" }),
                "Id",
                "Display");

            ViewData["DefaultMechanicId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic)
                    .OrderBy(e => e.FullName),
                "Id",
                "FullName");

            ViewBag.CurrentTuningGoal = (int)order.TuningGoal;

            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EstimatedHours,DepositPaid,VehicleId,DefaultMechanicId")] Order order, [FromForm] List<int> tuningGoals)
        {
            if (id != order.Id) return NotFound();

            order.TuningGoal = TuningGoal.None;
            foreach (var goal in tuningGoals)
                order.TuningGoal |= (TuningGoal)goal;

            if (!Order.IsValidTuningGoal(order.TuningGoal))
                ModelState.AddModelError("TuningGoal", "Można wybrać tylko jeden Stage (1, 2 lub 3).");

            if (ModelState.IsValid)
            {
                var existing = await _context.Orders.FindAsync(id);
                if (existing == null) return NotFound();

                existing.TuningGoal = order.TuningGoal;
                existing.EstimatedHours = order.EstimatedHours;
                existing.DepositPaid = order.DepositPaid;
                existing.VehicleId = order.VehicleId;
                existing.DefaultMechanicId = order.DefaultMechanicId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id });
            }

            ViewData["VehicleId"] = new SelectList(
                _context.Vehicles.Include(v => v.Client).OrderBy(v => v.Brand)
                    .Select(v => new {
                        v.Id,
                        Display = $"{v.Client.FullName} - {v.Brand} {v.Model} ({v.Year})"
                    }),
                "Id", "Display", order.VehicleId);

            ViewData["DefaultMechanicId"] = new SelectList(
                _context.Employees
                    .Where(e => e.Role == EmployeeRole.Mechanic)
                    .OrderBy(e => e.FullName),
                "Id", "FullName", order.DefaultMechanicId);

            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.DefaultMechanic)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST Orders/ChangeStatus/5
        // This action is used to change the status of an order without going through the edit form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.ChangeStatus(newStatus);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Status zlecenia został zmieniony na {newStatus}.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: /Orders/Invoices
        public async Task<IActionResult> Invoices()
        {
            var invoices = await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Vehicle)
                        .ThenInclude(v => v.Client)
                .OrderByDescending(i => i.IssuedAt)
                .ToListAsync();

            return View(invoices);
        }

        // GET: /Orders/WorkView/5
        [Authorize(Roles = "Mechanic")]
        public async Task<IActionResult> WorkView(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.DefaultMechanic)
                .Include(o => o.ServiceTasks)
                    .ThenInclude(st => st.AssignedEmployee)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .Include(o => o.WorkstationAssignments)
                    .ThenInclude(wa => wa.Workstation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            ViewBag.FreeWorkstations = await _context.Workstations
                .Where(w => !w.IsOccupied)
                .OrderBy(w => w.Type)
                .ToListAsync();

            return View(order);
        }

        // POST: /Orders/MarkPartUsed
        [HttpPost]
        [Authorize(Roles = "Owner,Mechanic")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPartUsed(
            int orderId, int partId, bool returnToWorkView = false)
        {
            var orderPart = await _context.OrderParts
                .Include(op => op.Part)
                .FirstOrDefaultAsync(op =>
                    op.OrderId == orderId && op.PartId == partId);

            if (orderPart == null) return NotFound();

            if (!orderPart.IsUsed)
            {
                orderPart.MarkAsUsed();
                await _context.SaveChangesAsync();
                TempData["Success"] =
                    $"Oznaczono {orderPart.Part?.Name} jako użytą.";
            }

            return returnToWorkView
                ? RedirectToAction("WorkView", "Orders", new { id = orderId })
                : RedirectToAction("Details", "Orders", new { id = orderId });
        }

        // GET: /Orders/GenerateInvoice/5
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.DefaultMechanic)
                .Include(o => o.ServiceTasks)
                    .ThenInclude(st => st.AssignedEmployee)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var invoice = await _context.Invoices
                .Include(i => i.Order)
                    .ThenInclude(o => o.Vehicle)
                        .ThenInclude(v => v.Client)
                .Include(i => i.Order)
                    .ThenInclude(o => o.OrderParts)
                        .ThenInclude(op => op.Part)
                .Include(i => i.Order)
                    .ThenInclude(o => o.ServiceTasks)
                .FirstOrDefaultAsync(i => i.OrderId == id);

            if (invoice == null)
            {
                invoice = new Invoice { OrderId = id, Order = order };
                invoice.GenerateFromOrder(order);
                _context.Invoices.Add(invoice);
                await _context.SaveChangesAsync();
            }

            invoice.Order = order;

            var pdf = _pdfService.GenerateInvoice(invoice);

            return File(pdf, "application/pdf",
                $"Faktura_{invoice.FormattedNumber.Replace("/", "-")}.pdf");
        }

        // GET: /Orders/GenerateTuningProtocol/5
        public async Task<IActionResult> GenerateTuningProtocol(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.DefaultMechanic)
                .Include(o => o.ServiceTasks)
                    .ThenInclude(st => st.AssignedEmployee)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var pdf = _pdfService.GenerateTuningProtocol(order);

            return File(pdf, "application/pdf",
                $"TuningProtocol_Zlecenie{id}.pdf");
        }
    }
}
