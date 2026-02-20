using MediatR;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.Nowy_folder
{
    public class GetElementsHandler : IRequestHandler<GetElementsQuery, ServiceResponse<ElementCacheDTO>>
    {
        private readonly IElementRepository _elementRepository;
        public GetElementsHandler(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }
        public async Task<ServiceResponse<ElementCacheDTO>> Handle(GetElementsQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementRepository.GetElementsQuery(), "Elements");
        }
    }
}
