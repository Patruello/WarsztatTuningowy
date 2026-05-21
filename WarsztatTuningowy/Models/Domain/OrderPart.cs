namespace WarsztatTuningowy.Models.Domain
{
    public class OrderPart
    {
        public int OrderId { get; set; }
        public int PartId { get; set; }

        public int Quantity { get; set; } = 1;
        public bool IsUsed { get; set; } = false;
        public bool IsVinLocked { get; set; } = false;
        public string? LockedVin { get; set; }

        public Order? Order { get; set; } = null!;
        public Part? Part { get; set; } = null!;

        public bool MarkAsUsed()
        {
            if (IsUsed) return false;

            if (Part == null) return false;

            if (Part.IsStockPart)
            {
                if (Part.Stock < Quantity)
                    return false;

                Part.Stock -= Quantity;
            }

            IsUsed = true;
            return true;
        }

        public decimal WholesaleCost() => Part.WholesalePrice * Quantity;
        public decimal RetailCost() => Part.RetailPrice * Quantity;

        public bool CanBeIssuedTo(string vin)
        {
            if (!IsVinLocked) return true;
            return LockedVin == vin;
        }
    }
}
