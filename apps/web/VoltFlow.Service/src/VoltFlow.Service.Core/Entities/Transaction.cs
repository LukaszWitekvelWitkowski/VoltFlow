using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public partial class Transaction
    {
        public int IdTransaction { get; set; }

        public int StockId { get; set; }           // FK (NOT NULL)
        public int? JobId { get; set; }             // FK (nullable)
        public int? SourceId { get; set; }          // FK -> Warehouse
        public int? TargetId { get; set; }          // FK -> Warehouse

        public int Quantity { get; set; }

        // ENUMY jako typ bazowy (EF)
        public TransactionType TransactionTypeId { get; set; }
        public CreateType CreatedById { get; set; }

        public DateTime Date { get; set; }

        // === NAVIGATION PROPERTIES ===

        public virtual Stock Stock { get; set; } = null!;

        public virtual Job? Job { get; set; }

        public virtual Warehouse? Source { get; set; }

        public virtual Warehouse? Target { get; set; }


        public ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();
    }

}
