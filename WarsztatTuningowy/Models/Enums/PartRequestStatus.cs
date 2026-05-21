using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Enums
{
    public enum PartRequestStatus
    {
        [Display(Name = "Oczekuje na zamówienie")]
        Pending,
        [Display(Name = "Zamówiono")]
         Ordered,
        [Display(Name = "Dostępne")]
        Ready
    }
}
