using MediatR;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Queries.Category
{
    public class GetCategoryByIdQuery : IRequest<ServiceResponse<CategoryDTO>>
    {
        public int Id { get; }
        public GetCategoryByIdQuery(int id)
        {
            Id = id;
        }
    }
}
