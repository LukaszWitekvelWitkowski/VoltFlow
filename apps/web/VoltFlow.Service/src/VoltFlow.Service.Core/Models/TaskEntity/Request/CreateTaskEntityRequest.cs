using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Models.TaskEntity.Request
{
    public class CreateTaskEntityRequest
    {
        public string Description { get; set; } = string.Empty;
        public WorkItemStatus Status { get; set; }
        public TaskEntityType TypeTask { get; set; }
    }
}
