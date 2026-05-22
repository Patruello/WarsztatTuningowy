using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarsztatTuningowy.Models.Domain
{
    public class Invoice
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Numer faktury")]
        public string Number { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.Now;
        [Required]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Kwota netto")]
        public decimal NetAmount { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Kwota VAT")]
        public decimal VatAmount { get; set; }
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Kwota brutto")]
        public decimal GrossAmount { get; set; }

        public int OrderId { get; set; }
        public Order? Order { get; set; }

        [NotMapped]
        public decimal VatRate => NetAmount > 0 ? VatAmount / NetAmount : 0;

        public void AssignNumber()
        {
            Number = $"FV/{IssuedAt:yyyy/MM}/{Id:D4}";
        }


        public void GenerateFromOrder(Order order, decimal vatRate = 0.23m)
        {
            NetAmount = order.TotalClientPrice();
            VatAmount = NetAmount * vatRate;
            GrossAmount = NetAmount + VatAmount;
        }

        public static string CsvHeader() => "Numer faktury;Data wystawienia;Klient;Netto (zł);VAT (zł);Brutto (zł)";

        public string ExportToCsv()
        {
            return $"{Number};{IssuedAt:yyyy-MM-dd};" +
                   $"{Order.Vehicle.Client.FullName};" +
                   $"{NetAmount};{VatAmount};{GrossAmount}";
        }

        public static string ExportAllToCsv(IEnumerable<Invoice> invoices)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine(CsvHeader());
            foreach (var invoice in invoices)
                sb.AppendLine(invoice.ExportToCsv());
            return sb.ToString();
        }
    }
}
