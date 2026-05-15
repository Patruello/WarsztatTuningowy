using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatTuningowy.Models.Domain
{
    public class ServiceTask
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa zadania jest wymagana")]
        [MaxLength(200)]
        [Display(Name = "Nazwa zadania")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Start")]
        public DateTime? StartTime { get; set; }
        [Display(Name = "Koniec")]
        public DateTime? EndTime { get; set; }
        [Display(Name = "Minuty przerwy")]
        public int PausedMinutes { get; set; }

        [Display(Name = "Notatki")]
        public string? Notes { get; set; }

        [Display(Name = "Zlecenie")]
        public int OrderId { get; set; }
        [Display(Name = "Przypisany pracownik")]
        public int? AssignedEmployeeId { get; set; }

        public Order? Order { get; set; }
        public Employee? AssignedEmployee { get; set; }

        [NotMapped]
        [Display(Name = "Czas pracy (min)")]
        public int TotalMinutes
        {
            get
            {
                if (StartTime == null || EndTime == null) return 0;
                return (int)(EndTime.Value - StartTime.Value).TotalMinutes - PausedMinutes;
            }
        }

        [NotMapped]
        [Display(Name = "Czas pracy")]
        public string TotalTime
        {
            get
            {
                if (StartTime == null || EndTime == null) return "Nie rozpoczęto";
                var hours = TotalMinutes / 60;
                var minutes = TotalMinutes % 60;
                return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
            }
        }

        public void StartTimer()
        {
            if (StartTime != null) return;
            StartTime = DateTime.Now;
        }

        public void StopTimer()
        {
            if (StartTime == null || EndTime != null) return;
            EndTime = DateTime.Now;
        }

        public void PauseTimer(int pauseMinutes)
        {
            if (StartTime == null || EndTime != null) return;
            PausedMinutes += pauseMinutes;
        }

        public void AddNote(string text)
        {
            var timestamp = DateTime.Now.ToString("dd-MM-yyyy HH:mm");
            var entry = $"[{timestamp}] {text}";
            Notes = string.IsNullOrEmpty(Notes)
                ? entry
                : $"{Notes}\n{entry}";
        }
    }                                           
}
