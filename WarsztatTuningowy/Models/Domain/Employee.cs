using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Models.Domain
{
    public class Employee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Imię i nazwisko jest wymagane")]
        [MaxLength(100)]
        [Display(Name = "Imię i nazwisko")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Rola jest wymagana")]
        [Display(Name = "Rola")]
        public EmployeeRole Role { get; set; }
        [Required(ErrorMessage = "Stawka godzinowa jest wymagana")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Stawka godzinowa (wewnętrzna)")]
        public decimal HourlyRateInternal { get; set; }
        [Required(ErrorMessage = "Stawka godzinowa jest wymagana")]
        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [Display(Name = "Stawka godzinowa (dla klienta)")]
        public decimal HourlyRateClient { get; set; }
        [MaxLength(450)]
        public string? UserId { get; set; }

        public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
        public ICollection<ServiceTask> ServiceTasks { get; set; } = new List<ServiceTask>();

        public decimal TotalWorkHours => ServiceTasks.Sum(t => t.TotalMinutes) / 60m;
    }
}
