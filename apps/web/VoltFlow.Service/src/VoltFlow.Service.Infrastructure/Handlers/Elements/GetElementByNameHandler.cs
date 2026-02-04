using MediatR;
using VoltFlow.Service.Application.Queries.Element;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models;
using VoltFlow.Service.Core.Models.Element;

namespace VoltFlow.Service.Infrastructure.Handlers.Elements
{
    public class GetElementByNameHandler : IRequestHandler<GetElementByNameQuery, ServiceResponse<ElementDTO>>
    {
        private readonly IElementRepository _elementRepository;

        public GetElementByNameHandler(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }
        public async Task<ServiceResponse<ElementDTO>> Handle(GetElementByNameQuery request, CancellationToken cancellationToken)
        {
            return ResponseValidator.EnsureSuccessAndData(await _elementRepository.GetElementByNameQuery(request.Name), "Element");
        }
    }
}
