using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Przyjęcie")]
        Received,
        [Display(Name = "Diagnoza")]
        Diagnosis,
        [Display(Name = "W trakcie realizacji")]
        InProgress,
        [Display(Name = "Kontrola jakości")]
        QualityCheck,
        [Display(Name = "Zakończone")]
        Completed
    }
}