using MediatR;
using VoltFlow.Service.Application.Queries.ElementGroup;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.ElementGroup
{
    public class GetElementGroupByIdHandler : IRequestHandler<GetElementGroupByIdQuery, ServiceResponse<ElementGroupDTO>>
    {

        private readonly IElementGroupRepository _elementGroupRepository;

        public GetElementGroupByIdHandler(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }

        public async Task<ServiceResponse<ElementGroupDTO>> Handle(GetElementGroupByIdQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementGroupRepository.GetElementGroupByIdQuery(request.Id), "Element Group");
        }
    }
}
