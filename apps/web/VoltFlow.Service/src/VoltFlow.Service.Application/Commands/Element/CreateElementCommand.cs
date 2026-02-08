using MediatR;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Application.Commands.Element
{
    public class CreateElementCommand :IRequest<ServiceResponse<ElementDTO>>
    {
        public string Name { get; set; }
        public CreateElementCommand(string name)
        {
            Name = name;
        }
    }
}
