using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class TransactionLog
    {
        public int IdTransactionLog { get; set; }

        public DateTime Timestamp { get; set; }

        // Na schemacie CreateBy [enum] - określa kto/co stworzyło wpis
        public ProcessType CreatedBy { get; set; }

        public string? Details { get; set; }

        // Na schemacie TransactionType [enum]
        public TransactionType TransactionType { get; set; }

        // Relacje
        public int TransactionId { get; set; }
        public Transaction EntityTransaction { get; set; } = null!;
    }
}
