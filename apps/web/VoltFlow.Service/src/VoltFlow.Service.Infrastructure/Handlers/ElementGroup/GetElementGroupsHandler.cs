using MediatR;
using VoltFlow.Service.Application.Queries.ElementGroup;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.ElementGroup
{
    public class GetElementGroupsHandler : IRequestHandler<GetElementGroupsQuery, ServiceResponse<ElementGroupCacheDTO>>
    {
        private readonly IElementGroupRepository _elementGroupRepository;
        public GetElementGroupsHandler(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }
        public async Task<ServiceResponse<ElementGroupCacheDTO>> Handle(GetElementGroupsQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementGroupRepository.GetElementGroupsQuery(), "Element Group");
        }
    }
}
