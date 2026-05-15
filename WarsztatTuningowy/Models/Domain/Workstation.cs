using System.ComponentModel.DataAnnotations;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Models.Domain
{
    public class Workstation
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa stanowiska jest wymagana")]
        [MaxLength(50)]
        [Display(Name = "Nazwa stanowiska")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Typ stanowiska jest wymagany")]
        [Display(Name = "Typ stanowiska")]
        public WorkstationType Type { get; set; }
        [Display(Name = "Czy zajęte?")]
        public bool IsOccupied { get; set; } = false;

        public ICollection<WorkstationAssignment> Assignments { get; set; } = new List<WorkstationAssignment>();

        public void Occupy()
        {
            IsOccupied = true;
        }

        public void Release()
        {
            IsOccupied = false;
        }
    }
}
