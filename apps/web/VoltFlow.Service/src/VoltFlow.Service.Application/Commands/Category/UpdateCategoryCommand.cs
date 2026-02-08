using MediatR;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Category.Request;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Category
{
    public class UpdateCategoryCommand : IRequest<ServiceResponse<CategoryDTO>>
    {
        public UpdateCategoryCommand(UpdateCategoryRequest request)
        {
            _request = request;
        }

        public UpdateCategoryRequest _request { get; }

    }
}
