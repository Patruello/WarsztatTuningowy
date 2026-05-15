using System.ComponentModel.DataAnnotations;
using WarsztatTuningowy.Models.Domain;

namespace WarsztatTuningowy.Models.Domain
{
    public class Client
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Imię i nazwisko jest wymagane")]
        [MaxLength(100, ErrorMessage = "Maksymalnie 100 znaków")]
        [Display(Name = "Imię i nazwisko")]
        public string FullName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Telefon jest wymagany")]
        [MaxLength(20)]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email jest wymagany")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Data rejestracji")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
