using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoltFlow.Service.Application.Commands.ElementGroup;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;

namespace VoltFlow.Service.Infrastructure.Handlers.ElementGroup
{
    public class CreateElementGroupHandler : IRequestHandler<CreateElementGroupCommand, ServiceResponse<ElementGroupDTO>>
    {
        private readonly IElementGroupRepository _elementGroupRepository;

        public CreateElementGroupHandler(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }

        public async Task<ServiceResponse<ElementGroupDTO>> Handle(CreateElementGroupCommand request, CancellationToken cancellationToken)
        {
            return await _elementGroupRepository.AddElementGroup(request._request);
        }
    }
}
