using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Models.TaskEntity.DTOs
{
    public class TaskEntityDTO
    {
        public int IdTask { get; set; }
        public string Description { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; }
        public TaskEntityType TypeTask { get; set; }
    }
}
