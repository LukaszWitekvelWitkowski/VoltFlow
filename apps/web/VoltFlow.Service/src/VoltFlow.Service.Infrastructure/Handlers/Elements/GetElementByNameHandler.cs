using MediatR;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Elements
{
    public class GetElementByNameHandler : IRequestHandler<GetElementByNameQuery, ServiceResponse<PagedResultDTO<ElementDTO>>>
    {
        private readonly IElementRepository _elementRepository;

        public GetElementByNameHandler(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }
        public async Task<ServiceResponse<PagedResultDTO<ElementDTO>>> Handle(GetElementByNameQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementRepository.GetElementsPagedByNameQuery(request.Name, request.PageNumber, request.PageSize), "Element");
        }
    }
}
