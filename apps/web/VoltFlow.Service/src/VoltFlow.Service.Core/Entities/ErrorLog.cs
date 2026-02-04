using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class ErrorLog
    {
        public int IdErrorLog { get; set; }
        public ProcessType ProcessId { get; set; }
        public LogLevel Level { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public ContextType ContextType { get; set; }
        public int ContextId { get; set; }
    }
}
