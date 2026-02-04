using MediatR;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

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
