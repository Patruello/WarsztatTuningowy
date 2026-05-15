using System.ComponentModel.DataAnnotations;

namespace WarsztatTuningowy.Models.Domain
{
    public class Vehicle
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "VIN jest wymagany")]
        [MinLength(17, ErrorMessage = "VIN musi mieć 17 znaków")]
        [MaxLength(17, ErrorMessage = "VIN musi mieć 17 znaków")]
        [Display(Name = "Numer VIN")]
        public string VIN { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required(ErrorMessage = "Marka jest wymagana")]
        [Display(Name = "Marka")]
        public string Brand { get; set; } = string.Empty;
        [MaxLength(50)]
        [Required(ErrorMessage = "Model jest wymagany")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;
        [Range(1900, 2100, ErrorMessage = "Podaj poprawny rok produkcji")]
        [Required(ErrorMessage = "Rok jest wymagany")]
        [Display(Name = "Rok")]
        public int Year { get; set; }
        [Required(ErrorMessage = "Typ silnika jest wymagany")]
        [MaxLength(100)]
        [Display(Name = "Typ silnika")]
        public string EngineType { get; set; } = string.Empty;
        [Required(ErrorMessage = "Typ ECU jest wymagany")]
        [MaxLength(100)]
        [Display(Name = "Typ ECU")]
        public string ECUType { get; set; } = string.Empty;
        [Display(Name = "Klient")]
        [Required(ErrorMessage = "Właściciel jest wymagany")]
        public int ClientId { get; set; }
        
        public Client? Client { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();

        public string TuningSpec => $"{Brand} {Model} ({Year}) | {EngineType}, ECU: {ECUType}";
    }
}
