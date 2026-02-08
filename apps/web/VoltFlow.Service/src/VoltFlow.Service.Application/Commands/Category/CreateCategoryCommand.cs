using MediatR;
using VoltFlow.Service.Core.Models.Category.DTOs;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Application.Commands.Category
{
    public class CreateCategoryCommand : IRequest<ServiceResponse<CategoryDTO>>
    {
        public string Name { get; }
        public CreateCategoryCommand(string name)
        {
            Name = name;
        }
    }
}
