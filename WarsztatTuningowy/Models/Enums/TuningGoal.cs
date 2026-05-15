using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Enums
{
    [Flags]
    public enum TuningGoal
    {
        None = 0,
        Stage1 = 1,
        Stage2 = 2,
        Stage3 = 4,
        [Display(Name = "Mechaniczne")]
        Mechanical = 8,
        [Display(Name = "Wizualne")]
        Visual = 16
    }
}