using MediatR;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Models.Category
{
    public class GetCategoryByNameQuery : PaginationParams, IRequest<ServiceResponse<PagedResultDTO<CategoryDTO>>>
    {
        public string Name { get; }
        public GetCategoryByNameQuery(string? name, int number, int size)
        {
            Name = name;
            PageNumber = number;
            PageSize = size;    
        }
    }
}
