using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatTuningowy.Models.Domain
    {
    public class WorkstationAssignment
    {
        public int Id { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.Now;
        public DateTime? ReleasedAt { get; set; }
        public string? Notes { get; set; }

        public int OrderId { get; set; }
        public int WorkstationId { get; set; }

        public Order? Order { get; set; } = null!;
        public Workstation? Workstation { get; set; } = null!;

        public void Release()
        {
            ReleasedAt = DateTime.Now;
            Workstation.Release();
        }

        [NotMapped]
        public bool IsActive => ReleasedAt == null;

        [NotMapped]
        public int DurationMinutes
        {
            get
            {
                var endTime = ReleasedAt ?? DateTime.Now;
                return (int)(endTime - AssignedAt).TotalMinutes;
            }
        }
    }
}