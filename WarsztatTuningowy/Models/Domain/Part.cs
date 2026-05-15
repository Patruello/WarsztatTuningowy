using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatTuningowy.Models.Domain
{
    public class Part
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nazwa części jest wymagana")]
        [MaxLength(200, ErrorMessage = "Nazwa części nie może być dłuższa niż 200 znaków")]
        [Display(Name = "Nazwa części")]
        public string Name { get; set; } = string.Empty;
        [Range(0.01, 999999, ErrorMessage = "Cena musi być większa od 0")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cena hurtowa (netto)")]
        public decimal WholesalePrice { get; set; }
        [Range(0.01, 999999, ErrorMessage = "Cena musi być większa od 0")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cena detaliczna (brutto)")]
        public decimal RetailPrice { get; set; }
        [Required(ErrorMessage = "Stan magazynowy jest wymagany")]
        [Range(0, int.MaxValue, ErrorMessage = "Stan magazynowy nie może być ujemny")]
        [Display(Name = "Stan magazynowy")]
        public int Stock { get; set; } = 0;
        [Required(ErrorMessage = "Minimalny stan magazynowy jest wymagany")]
        [Range(0, int.MaxValue, ErrorMessage = "Minimalny stan magazynowy nie może być ujemny")]
        [Display(Name = "Minimalny stan magazynowy")]
        public int MinStock { get; set; } = 0;
        [MaxLength(200, ErrorMessage = "Nazwa dostawcy nie może być dłuższa niż 200 znaków")]
        [Display(Name = "Dostawca")]
        public string SupplierName { get; set; } = string.Empty;
        public ICollection<OrderPart> OrderParts { get; set; } = new List<OrderPart>();

        public bool IsLowStock() => Stock < MinStock;

        public decimal Margin() => RetailPrice - WholesalePrice;

        public bool AdjustStock(int quantity)
        {
            if (Stock + quantity < 0)
                return false;

            Stock += quantity;
            return true;
        }
    }
}
