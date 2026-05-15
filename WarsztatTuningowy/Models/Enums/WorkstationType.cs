using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Enums
{
    public enum WorkstationType
    {
        [Display(Name = "Podnośnik")]
        Lift,
        [Display(Name = "Hamownia")]
        Dyno,
        [Display(Name = "Stanowisko diagnostyczne")]
        Diagnostic,
        [Display(Name = "Detailing")]
        Detailing,
        [Display(Name = "Spawanie")]
        Welding
    }
}