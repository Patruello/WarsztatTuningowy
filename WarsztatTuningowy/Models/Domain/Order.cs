using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Models.Domain
{
    public class Order
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Cel tuningu jest wymagany")]
        [Display(Name = "Cel tuningu")]
        public TuningGoal TuningGoal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Received;
        [Required(ErrorMessage = "Szacunkowa liczba godzin jest wymagana")]
        [Column(TypeName = "decimal(8, 2)")]
        [Range(0.5, 400, ErrorMessage = "Podaj szacunkową liczbę godzin (0.5 - 400)")]
        [Display(Name = "Szacunkowe czas (h)")]
        public decimal EstimatedHours { get; set; }
        [Display(Name = "Zaliczka wpłacona")]
        public bool DepositPaid { get; set; }
        [Display(Name = "Data utworzenia")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Display(Name = "Pojazd")]
        public int VehicleId { get; set; }
        [Display(Name = "Domyślny mechanik")]
        public int? DefaultMechanicId { get; set; }
        
        [Display(Name = "Pojazd")]
        public Vehicle? Vehicle { get; set; }
        [Display(Name = "Domyślny mechanik")]
        public Employee? DefaultMechanic { get; set; }
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();
        public ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();
        public ICollection<WorkstationAssignment> WorkstationAssignments { get; set; } = new List<WorkstationAssignment>();
        public ICollection<PartRequest> PartRequests { get; set; } = new List<PartRequest>();
        public Invoice? Invoice { get; set; }

        [NotMapped]
        public decimal ActualHours => ServiceTasks.Sum(st => st.TotalMinutes) / 60.0m;
        [NotMapped]
        public bool IsOverTime => ActualHours > EstimatedHours;
        [NotMapped]
        public decimal OvertimeHours => Math.Max(0, ActualHours - EstimatedHours);

        public decimal TotalWorkshopCost()
        {
            var partsCost = OrderParts
                .Where(op => op.IsUsed)
                .Sum(op => op.WholesaleCost());

            var laborCost = ServiceTasks
                .Sum(st => (st.TotalMinutes / 60m) * (st.AssignedEmployee?.HourlyRateInternal ?? 0));

            return partsCost + laborCost;
        }

        public decimal TotalClientPrice()
        {
            var partsCost = OrderParts
                .Where(op => op.IsUsed)
                .Sum(op => op.RetailCost());

            var laborCost = ServiceTasks
                .Sum(st => (st.TotalMinutes / 60m)
                    * (st.AssignedEmployee?.HourlyRateClient ?? 0));

            return partsCost + laborCost;
        }

        public decimal Margin()
        {
            return TotalClientPrice() - TotalWorkshopCost();
        }

        public void ChangeStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public bool IsReadyForTuning()
        {
            if (PartRequests.Any(pr => pr.Status != PartRequestStatus.Ready)) return false;

            return OrderParts
                .Where(op => !op.IsUsed)
                .All(op => op.Part != null 
                    && (!op.Part.IsStockPart || op.Part.Stock >= op.Quantity));
        }

        public static bool IsValidTuningGoal(TuningGoal goal)
        {
            var stages = TuningGoal.Stage1 | TuningGoal.Stage2 | TuningGoal.Stage3;
            var selectedStages = goal & stages;
            return selectedStages == 0
                || selectedStages == TuningGoal.Stage1
                || selectedStages == TuningGoal.Stage2
                || selectedStages == TuningGoal.Stage3;
        }
    }
}
