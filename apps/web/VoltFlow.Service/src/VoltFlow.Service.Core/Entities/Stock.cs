namespace VoltFlow.Service.Core.Entities
{
    public class Stock
    {
        public int IdStock { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
        public long RowVersion { get; set; }
        public int WarehouseId { get; set; }

        // Relacje
        public int ElementId { get; set; }
        public Element Element { get; set; } = null!;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public Warehouse Warehouses { get; set; } = null!;
    }
}
