using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class EventJobLog
    {
        public int IdEventJobLog { get; set; }

        public string Status { get; set; } = string.Empty; // Lub Enum, jeśli to status techniczny
        public DateTime Timestamp { get; set; }
        public NotificationStatus NotificationStatus { get; set; }

        // Na schemacie widzę ProcessId [enum] i TypeEvent [enum]
        public ProcessType ProcessId { get; set; }
        public EventType TypeEvent { get; set; }

        // Relacje
        public int EventJobId { get; set; }
        public EventJob EventJob { get; set; } = null!;
    }
}
