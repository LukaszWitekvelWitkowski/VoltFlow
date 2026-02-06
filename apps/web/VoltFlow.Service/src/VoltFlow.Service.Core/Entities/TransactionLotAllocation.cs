namespace VoltFlow.Service.Core.Entities
{
    public class TransactionLotAllocation
    {
        public int IdAllocation { get; set; }
        public int TransactionId { get; set; }
        public int PurchaseLotId { get; set; }
        public int QuantityTaken { get; set; }
        public decimal UnitNetCost { get; set; }

        // Nawigacja
        public virtual PurchaseLot PurchaseLot { get; set; } = null!;
        // Zakładam, że masz encję Transaction
        public virtual Transaction Transaction { get; set; } = null!;
    }
}
