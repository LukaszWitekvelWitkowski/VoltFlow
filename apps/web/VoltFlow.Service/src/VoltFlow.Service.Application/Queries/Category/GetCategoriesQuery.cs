using MediatR;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Category;

namespace VoltFlow.Service.Application.Queries.Category
{
    public class GetCategoriesQuery : IRequest<ServiceResponse<CategoriesDTO>>
    {
    }
}


