using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Models.Enums;
using WarsztatTuningowy.Models.ViewModels;

namespace WarsztatTuningowy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine($"=== INDEX ===");
            System.Diagnostics.Debug.WriteLine($"IsAuthenticated: {User.Identity?.IsAuthenticated}");
            System.Diagnostics.Debug.WriteLine($"IsInRole Owner: {User.IsInRole("Owner")}");
            System.Diagnostics.Debug.WriteLine($"IsInRole Mechanic: {User.IsInRole("Mechanic")}");
            System.Diagnostics.Debug.WriteLine($"Claims: {string.Join(", ", User.Claims.Select(c => $"{c.Type}={c.Value}"))}");

            if (User.IsInRole("Owner"))
                return RedirectToAction("Owner");

            if (User.IsInRole("Mechanic"))
                return RedirectToAction("Mechanic");

            if (User.IsInRole("Storekeeper"))
                return RedirectToAction("Storekeeper");

            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Owner()
        {
            var vm = await BuildOwnerDashboard();
            return View(vm);
        }

        [Authorize(Roles = "Mechanic")]
        public async Task<IActionResult> Mechanic()
        {
            var userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (employee == null) return NotFound();

            var orders = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.ServiceTasks)
                    .ThenInclude(st => st.AssignedEmployee)
                .Where(o => o.DefaultMechanicId == employee.Id
                    || o.ServiceTasks.Any(st =>
                        st.AssignedEmployeeId == employee.Id))
                .Where(o => o.Status != OrderStatus.Completed)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            ViewBag.Employee = employee;
            return View(orders);
        }

        [Authorize(Roles = "Storekeeper")]
        public async Task<IActionResult> Storekeeper()
        {
            var parts = await _context.Parts
                .Include(p => p.OrderParts)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(parts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        private async Task<DashboardViewModel> BuildOwnerDashboard()
        {
            var allOrders = await _context.Orders
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Client)
                .Include(o => o.DefaultMechanic)
                .Include(o => o.ServiceTasks)
                .Include(o => o.OrderParts)
                    .ThenInclude(op => op.Part)
                .ToListAsync();

            var activeOrders = allOrders
                .Where(o => o.Status != OrderStatus.Completed)
                .OrderBy(o => o.CreatedAt)
                .ToList();

            var workstations = await _context.Workstations
                .Include(w => w.Assignments)
                    .ThenInclude(a => a.Order)
                        .ThenInclude(o => o.Vehicle)
                .OrderBy(w => w.Type)
                .ToListAsync();

            var lowStockParts = await _context.Parts
                .Where(p => p.Stock <= p.MinStock)
                .OrderBy(p => p.Stock)
                .ToListAsync();

            var mechanics = await _context.Employees
                .Where(e => e.Role == EmployeeRole.Mechanic)
                .Include(e => e.ServiceTasks)
                .Include(e => e.AssignedOrders)
                .OrderBy(e => e.FullName)
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalOrders = allOrders.Count,
                ActiveOrders = activeOrders.Count,
                CompletedOrders = allOrders.Count(o =>
                    o.Status == OrderStatus.Completed),
                TotalClients = await _context.Clients.CountAsync(),
                TotalVehicles = await _context.Vehicles.CountAsync(),
                LowStockParts = lowStockParts,
                LowStockPartsCount = lowStockParts.Count,
                OccupiedWorkstations = workstations.Count(w => w.IsOccupied),
                FreeWorkstations = workstations.Count(w => !w.IsOccupied),
                Workstations = workstations,
                ActiveOrdersList = activeOrders,
                Mechanics = mechanics,
                RecentOrders = allOrders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList()
            };
        }
    }
}