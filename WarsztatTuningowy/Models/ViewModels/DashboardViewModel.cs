using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int ActiveOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int TotalClients { get; set; }
        public int TotalVehicles { get; set; }

        public int LowStockPartsCount { get; set; }
        public List<Part> LowStockParts { get; set; } = new();

        public int OccupiedWorkstations { get; set; }
        public int FreeWorkstations { get; set; }
        public List<Workstation> Workstations { get; set; } = new();

        public List<Order> ActiveOrdersList { get; set; } = new();

        public List<Employee> Mechanics { get; set; } = new();

        public List<Order> RecentOrders { get; set; } = new();

        public int OvertimeOrdersCount { get; set; }
    }
}
