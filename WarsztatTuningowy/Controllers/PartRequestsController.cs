using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarsztatTuningowy.Data;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Controllers
{
    [Authorize]
    public class PartRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /PartRequests/Create?orderId=5
        [Authorize(Roles = "Owner,Mechanic")]
        public async Task<IActionResult> Create(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }
                
            ViewBag.Order = order;
            ViewBag.Parts = await _context.Parts
                .OrderBy(p => p.Name)
                .ToListAsync();

            return View(new PartRequest { OrderId = orderId });
        }

        // POST: /PartRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Mechanic")]
        public async Task<IActionResult> Create(
            int orderId,
            int? partId,
            string? customPartName,
            int quantity,
            string? notes,
            bool returnToWorkView = false)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) 
            {
                return NotFound();
            }

            if (!order.DepositPaid)
            {
                TempData["Error"] = "Nie można dodawać części do zlecenia, dopóki nie zostanie opłacona zaliczka.";
                return returnToWorkView
                    ? RedirectToAction("WorkView", "Orders", new { id = orderId })
                    : RedirectToAction("Details", "Orders", new { id = orderId });
            }

            if (partId == null && string.IsNullOrEmpty(customPartName))
            {
                TempData["Error"] = "Wybierz cześć z katologu lub podaj nazwę.";
                return RedirectToAction("Create", new { orderId = orderId });
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);

            if (partId.HasValue)
            {
                var part = await _context.Parts
                    .Include(p => p.OrderParts)
                    .FirstOrDefaultAsync(p => p.Id == partId.Value);
                if (part != null && part.IsStockPart && part.CanReserve(quantity))
                {
                    var existing = await _context.OrderParts
                        .FirstOrDefaultAsync(op => op.OrderId == orderId && op.PartId == partId.Value);

                    if (existing != null)
                    {
                        TempData["Error"] = "Ta część jest już dodana do tego zlecenia.";
                    }
                    else
                    {
                        var orderPart = new OrderPart
                        {
                            OrderId = orderId,
                            PartId = part.Id,
                            Quantity = quantity,
                            IsUsed = false
                        };

                        _context.OrderParts.Add(orderPart);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = $"{part.Name} dodano do zlecenia.";
                    }

                    return returnToWorkView
                        ? RedirectToAction("WorkView", "Orders", new { id = orderId })
                        : RedirectToAction("Details", "Orders", new { id = orderId });
                }

                var request = new PartRequest
                {
                    OrderId = orderId,
                    PartId = partId,
                    Quantity = quantity,
                    Notes = notes,
                    Status = PartRequestStatus.Pending,
                    RequestedByEmployeeId = employee?.Id,
                };
                _context.PartRequests.Add(request);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Złożono prośbę o {(await _context.Parts.FindAsync(partId))?.Name}. Magazynier zostanie poinformowany.";
            }
            else
            {
                var request = new PartRequest
                {
                    OrderId = orderId,
                    CustomPartName = customPartName,
                    Quantity = quantity,
                    Notes = notes,
                    Status = PartRequestStatus.Pending,
                    RequestedByEmployeeId = employee?.Id,
                };
                _context.PartRequests.Add(request);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Złożono prośbę o {customPartName}. Magazynier zostanie poinformowany.";
            }

            return returnToWorkView
                ? RedirectToAction("WorkView", "Orders", new { id = orderId })
                : RedirectToAction("Details", "Orders", new { id = orderId });
        }

        // POST: /PartRequests/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Storekeeper")]
        public async Task<IActionResult> UpdateStatus(int id, PartRequestStatus newStatus)
        {
            var request = await _context.PartRequests
                .Include(pr => pr.Part)
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            if (newStatus == PartRequestStatus.Ready)
            {
                if (request.PartId.HasValue)
                {
                    var part = await _context.Parts.FindAsync(request.PartId);

                    if (part == null)
                    {
                        TempData["Error"] = "Nie można oznaczyć jako gotowe - część nie istnieje w katalogu.";
                        return RedirectToAction("Index", "Home");
                    }

                    if (part.Stock < request.Quantity)
                    {
                        TempData["Error"] = $"Nie można oznaczyć jako gotowe — brak wystarczającego stanu magazynowego. " +
                            $"Dostępne: {part.Stock} szt., wymagane: {request.Quantity} szt.";
                        return RedirectToAction("Index", "Home");
                    }

                    var existing = await _context.OrderParts
                        .FirstOrDefaultAsync(op =>
                            op.OrderId == request.OrderId &&
                            op.PartId == request.PartId);

                    if (existing == null)
                    {
                        _context.OrderParts.Add(new OrderPart
                        {
                            OrderId = request.OrderId,
                            PartId = part.Id,
                            Quantity = request.Quantity,
                            IsUsed = false
                        });
                    }

                    request.Status = PartRequestStatus.Ready;
                }
                else
                {
                    TempData["Error"] = $"Nie można oznaczyć jako gotowe — część '{request.CustomPartName}' nie ma wpisu " +
                        $"w katalogu. Najpierw dodaj część do magazynu i przypisz ją do tego zapotrzebowania.";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                request.Status = newStatus;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status zapotrzebowania zmieniony na {newStatus}.";

            return RedirectToAction("Index", "Home");
        }

        // POST: /PartRequests/AssignPart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Owner,Storekeeper")]
        public async Task<IActionResult> AssignPart(int id, int partId)
        {
            var request = await _context.PartRequests
                .FirstOrDefaultAsync(pr => pr.Id == id);

            if (request == null)
            {
                return NotFound();
            }

            request.PartId = partId;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Część przypisana — możesz teraz oznaczyć jako Gotowe.";
            return RedirectToAction("Index", "Home");
        }
    }
}
