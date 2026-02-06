namespace VoltFlow.Service.Core.Entities
{
    public class PurchaseLot
    {
        public int IdPurchaseLot { get; set; }
        public int ElementId { get; set; }
        public decimal UnitNetCost { get; set; }
        public int QuantityRemaining { get; set; }
        public DateTime PurchasedAt { get; set; }

        // Nawigacja
        public virtual ICollection<TransactionLotAllocation> TransactionLotAllocations { get; set; } = new List<TransactionLotAllocation>();
    }
}
