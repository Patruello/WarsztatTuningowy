using System.ComponentModel.DataAnnotations;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Models.Domain
{
    public class PartRequest
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }
        public int? PartId { get; set; }
        [Display(Name = "Nazwa części")]
        public string? CustomPartName { get; set; } //jeśli część nie jest w katalogu
        [Required]
        [Range(1, 999, ErrorMessage = "Podaj ilość (1-999)")]
        public int Quantity { get; set; }
        [Display(Name = "Notatka")]
        public string? Notes { get; set; }

        public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public int? RequestedByEmployeeId { get; set; }

        public Order? Order { get; set; }
        public Part? Part { get; set; }
        public Employee? RequestedByEmployee { get; set; }

        public string DisplayName => Part?.Name ?? CustomPartName ?? "-";
    }
}
