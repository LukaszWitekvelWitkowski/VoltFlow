using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class TaskEntity
    {
        public int IdTask { get; set; }
        public string Description { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; }
        public TaskType TypeTask { get; set; }

        // Relacje
        public int JobId { get; set; }
        public Job Job { get; set; } = null!;
    }
}
