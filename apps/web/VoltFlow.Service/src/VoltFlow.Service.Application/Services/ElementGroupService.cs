using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.ElementGroup.DTOs;
using VoltFlow.Service.Core.Models.ElementGroup.Request;

namespace VoltFlow.Service.Application.Services
{
    public class ElementGroupService : IElementGroupService
    {
        private readonly IElementGroupRepository _elementGroupRepository;

        public ElementGroupService(IElementGroupRepository elementGroupRepository)
        {
            _elementGroupRepository = elementGroupRepository;
        }

        public async Task<ServiceResponse<ElementGroupDTO>> CreateElementGroup(CreateElementGroupRequest request)
        {
            var validation = ValidateName(request.Name);
            if (!validation._IsSuccess) return validation;

            var duplicateCheck = await CheckForDuplicateName(request.Name);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _elementGroupRepository.AddElementGroup(request);
        }

        public async Task<ServiceResponse<ElementGroupDTO>> UpdateElementGroup(UpdateElementGroupRequest request)
        {
            var validation = ValidateName(request.Name);
            if (!validation._IsSuccess) return validation;

            var allGroupsResponse = await _elementGroupRepository.GetElementGroupsQuery();
            var currentGroup = allGroupsResponse._Data?.ElementGroups
                .FirstOrDefault(g => g.IdElementGroup == request.IdElementGroup);

            if (currentGroup == null)
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Grupa elementów nie istnieje.", 404);
            }

            if (IsDataUnchanged(currentGroup, request))
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Nie wprowadzono żadnych zmian.", 200);
            }

            var duplicateCheck = await CheckForDuplicateName(request.Name, request.IdElementGroup);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _elementGroupRepository.UpdateElementGroup(request);
        }

        #region Private Helper Methods

        private ServiceResponse<ElementGroupDTO> ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Nazwa grupy nie może być pusta.", 400);
            }
            return ServiceResponse<ElementGroupDTO>.Result(null!);
        }

        private async Task<ServiceResponse<ElementGroupDTO>> CheckForDuplicateName(string name, int? excludeId = null)
        {
            var allGroupsResponse = await _elementGroupRepository.GetElementGroupsQuery();

            var exists = allGroupsResponse._Data?.ElementGroups
                .Any(g => g.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)
                          && (excludeId == null || g.IdElementGroup != excludeId));

            if (exists == true)
            {
                return ServiceResponse<ElementGroupDTO>.Failure("Grupa o podanej nazwie już istnieje.", 409);
            }

            return ServiceResponse<ElementGroupDTO>.Result(null!);
        }

        private bool IsDataUnchanged(ElementGroupDTO current, UpdateElementGroupRequest request)
        {
            return current.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete
                   && current.CategoryId == request.CategoryId;
        }

        #endregion
    }
}
