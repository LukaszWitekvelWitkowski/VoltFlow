using MediatR;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Elements
{
    public class GetElementSearchHandler : IRequestHandler<GetElementSearchQuery, ServiceResponse<PagedResultDTO<ElemntGroupDTO>>>
    {
        private readonly IElementRepository _elementRepository;

        public GetElementSearchHandler(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }
        public async Task<ServiceResponse<PagedResultDTO<ElemntGroupDTO>>> Handle(GetElementSearchQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementRepository.GetElementsPagedByNameQuery(request.Name, request.PageNumber, request.PageSize), "Element");
        }
    }
}
