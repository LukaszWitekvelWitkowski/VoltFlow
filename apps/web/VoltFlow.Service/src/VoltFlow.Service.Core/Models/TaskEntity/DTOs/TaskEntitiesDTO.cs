using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.TaskEntity.DTOs
{
    public class TaskEntitiesDTO : ICacheData<TaskEntityDTO>
    {
        public IEnumerable<TaskEntityDTO> Items { get; set; } = new List<TaskEntityDTO>();

        public void insert(IEnumerable<TaskEntityDTO> enumerable)
        {
          Items = enumerable;
        }
    }
}
