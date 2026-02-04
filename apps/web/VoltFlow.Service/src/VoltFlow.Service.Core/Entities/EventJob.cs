using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class EventJob
    {
        public int IdEventJob { get; set; }

        // Na schemacie widzę pole Status [enum]
        public JobStatus Status { get; set; }

        public string EventDetails { get; set; } = string.Empty;

        // Na schemacie widzę pole TypeEvent [enum]
        public EventType TypeEvent { get; set; }

        // Relacje
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        // Historia zmian dla tego zdarzenia
        public ICollection<EventJobLog> Logs { get; set; } = new List<EventJobLog>();
    }
}
