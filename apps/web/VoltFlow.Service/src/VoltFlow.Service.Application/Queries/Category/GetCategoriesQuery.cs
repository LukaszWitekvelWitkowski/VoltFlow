using MediatR;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Queries.Category
{
    public class GetCategoriesQuery : IRequest<ServiceResponse<CategoriesDTO>>
    {
    }
}


