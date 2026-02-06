using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class Warehouse
    {
        public int IdWarehouse { get; set; }
        public string Name { get; set; } = string.Empty;

        // Używamy Enuma dla pola Location [enum]
        public LocationType Location { get; set; }
        public bool IsObsolete { get; set; }

        // Relacje
        public int? JobId { get; set; }
        public Job? Job { get; set; }

        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();

        public virtual ICollection<Transaction> OutgoingTransactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Transaction> IncomingTransactions { get; set; } = new List<Transaction>();
    }
}
