using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Application.Services
{
    public class ElementGroupService : IElementGroupService
    {
        private readonly IElementGroupRepository _elementGroupRepository;

        public ElementGroupService(IElementGroupRepository elementGroupRepository) => _elementGroupRepository = elementGroupRepository;

        public async Task<ServiceResponse<ElementGroupDTO>> CreateElementGroup(CreateElementGroupRequest request)
        {
            // 1. validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa grupy nie może być pusta.");

            // 2. Duplicate checking via dedicated repository method
            if (await _elementGroupRepository.IsExists(request.Name))
                throw new ConflictException("Grupa o podanej nazwie już istnieje.");

            // 3. Save
            var result = await _elementGroupRepository.AddElementGroup(request);
            return ServiceResponse<ElementGroupDTO>.Success(result._Data!);
        }

        public async Task<ServiceResponse<ElementGroupDTO>> UpdateElementGroup(UpdateElementGroupRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa grupy nie może być pusta.");

            // 1. Getting the current state
            var currentResponse = await _elementGroupRepository.GetElementGroupByIdQuery(request.IdElementGroup);
            var currentGroup = currentResponse._Data ?? throw new NotFoundException("Grupa elementów nie istnieje.");

            // 2. idempotence (whether the data has actually changed)
            if (IsDataUnchanged(currentGroup, request))
                return ServiceResponse<ElementGroupDTO>.Success(currentGroup);

            // 3. Duplicate check (excluding the currently edited record)
            if (await _elementGroupRepository.IsExists(request.Name, request.IdElementGroup))
                throw new ConflictException("Grupa o podanej nazwie już istnieje.");

            // 4. Update
            var updated = await _elementGroupRepository.UpdateElementGroup(request);
            return ServiceResponse<ElementGroupDTO>.Success(updated._Data!);
        }

        #region Private Helper Methods

        private bool IsDataUnchanged(ElementGroupDTO current, UpdateElementGroupRequest request)
        {
            return current.Name.Trim().Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete
                   && current.CategoryId == request.CategoryId;
        }

        #endregion
    }
}
