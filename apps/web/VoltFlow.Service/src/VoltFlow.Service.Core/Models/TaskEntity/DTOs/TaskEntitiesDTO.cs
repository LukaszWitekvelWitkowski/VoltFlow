namespace VoltFlow.Service.Core.Models.TaskEntity.DTOs
{
    public class TaskEntitiesDTO
    {
        public IEnumerable<TaskEntityDTO> TaskEntities { get; set; }

        public TaskEntitiesDTO(IEnumerable<TaskEntityDTO>? taskEntities)
        {
            if (taskEntities is null)
            {
                TaskEntities = new List<TaskEntityDTO>();
                return;
            }
            TaskEntities = taskEntities;
        }
    }
}
