using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoltFlow.Service.Application.Queries.ElementGroup;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.ElementGroup
{
    public class GetElementGroupSearchHandler : IRequestHandler<GetElementGroupSearchQuery, ServiceResponse<PagedResultDTO<ElementGroupDTO>>>
    {
        private readonly IElementGroupRepository _elementGroupRepository;

        public GetElementGroupSearchHandler(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }

        public async Task<ServiceResponse<PagedResultDTO<ElementGroupDTO>>> Handle(GetElementGroupSearchQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementGroupRepository.GetElementGroupSearchQuery(request.Name, request.PageNumber, request.PageSize), "ElementGroup");
        }
    }
}
