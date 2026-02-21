using MediatR;
using VoltFlow.Service.Application.Commands.ElementGroup;
using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.Validators;

namespace VoltFlow.Service.Infrastructure.Handlers.ElementGroup
{
    public class UpdateElementGroupHandler : IRequestHandler<UpdateElementGroupCommand, ServiceResponse<ElementGroupDTO>>
    {
        private readonly IElementGroupRepository _elementGroupRepository;

        public UpdateElementGroupHandler(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }

        public async Task<ServiceResponse<ElementGroupDTO>> Handle(UpdateElementGroupCommand request, CancellationToken cancellationToken)
        {

            return await ResponseValidator.ExecuteAsync(async () => await _elementGroupRepository.UpdateElementGroup(request._request), "ElementGroup");
        }
    }
}
