using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Enums
{
    public enum  EmployeeRole
    {
        [Display(Name = "Właściciel")]
        Owner,
        [Display(Name = "Mechanik")]
        Mechanic,
        [Display(Name = "Magazynier")]
        Storekeeper
    }
}