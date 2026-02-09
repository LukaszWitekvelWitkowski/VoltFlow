using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoltFlow.Service.Application.Commands.Element;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.Elements
{
    public class CreateElementHandler : IRequestHandler<CreateElementCommand, ServiceResponse<ElementDTO>>
    {
        private readonly IElementService _elementService;

        public CreateElementHandler(IElementService elementService)
        {
            _elementService = elementService;
        }

        public async Task<ServiceResponse<ElementDTO>> Handle(CreateElementCommand request, CancellationToken cancellationToken)
        {
            return await _elementService.CreateElement(request._request);
        }
    }
}
