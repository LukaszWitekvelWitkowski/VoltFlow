using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.TaskEntity.DTOs;

namespace VoltFlow.Service.Application.Queries.TaskEntity
{
    public class GetTaskEntitySearchQuery : PaginationParams, IRequest<ServiceResponse<PagedResultDTO<TaskEntityDTO>>>
    {
        public string? Name { get; }
        public GetTaskEntitySearchQuery(string? name, int number, int size)
        {
            Name = name;
            PageNumber = number;
            PageSize = size;
        }
    }
}
