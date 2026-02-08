using MediatR;
using VoltFlow.Service.Application.Commands.Element;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.Elements
{
    public class UpdateElementHandler : IRequestHandler<UpdateElementCommand, ServiceResponse<ElementDTO>>
    {
        private readonly IElementService _elementService;
        public UpdateElementHandler(IElementService elementService)
        {
            _elementService = elementService;
        }
        public async Task<ServiceResponse<ElementDTO>> Handle(UpdateElementCommand request, CancellationToken cancellationToken)
        {
            return await _elementService.UpdateElement(request.Request);
        }
    }
}
