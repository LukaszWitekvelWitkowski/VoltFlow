using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Application.Queries.TaskEntity
{
    public class GetTaskEntityByIdQuery : IRequest<ServiceResponse<TaskEntityDTO>>
    {
        public int Id { get; }
        public GetTaskEntityByIdQuery(int id)
        {
            Id = id;
        }
    }
}
