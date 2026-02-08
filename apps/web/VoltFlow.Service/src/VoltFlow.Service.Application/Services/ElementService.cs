using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Application.Services
{
    public class ElementService : IElementService
    {
        private readonly IElementRepository _elementRepository;

        public ElementService(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }

        public async Task<ServiceResponse<ElementDTO>> CreateElement(CreateElementRequest request)
        {
            var validation = ValidateName(request.Name);
            if (!validation._IsSuccess) return validation;

            var duplicateCheck = await CheckForDuplicateName(request.Name);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _elementRepository.AddElement(request);
        }

        public async Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request)
        {
            var validation = ValidateName(request.Name);
            if (!validation._IsSuccess) return validation;

            var allElementsResponse = await _elementRepository.GetElementsQuery();
            var currentElement = allElementsResponse._Data?.Elements
                .FirstOrDefault(e => e.IdElement == request.Id);

            if (currentElement == null)
            {
                return ServiceResponse<ElementDTO>.Failure("Element nie istnieje.", 404);
            }

            if (IsDataUnchanged(currentElement, request))
            {
                return ServiceResponse<ElementDTO>.Failure("Nie wprowadzono żadnych zmian.", 200);
            }

            var duplicateCheck = await CheckForDuplicateName(request.Name, request.Id);
            if (!duplicateCheck._IsSuccess) return duplicateCheck;

            return await _elementRepository.UpdateElement(request);
        }

        #region Private Helper Methods

        private ServiceResponse<ElementDTO> ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResponse<ElementDTO>.Failure("Nazwa elementu nie może być pusta.", 400);
            }
            return ServiceResponse<ElementDTO>.Result(null!);
        }

        private async Task<ServiceResponse<ElementDTO>> CheckForDuplicateName(string name, int? excludeId = null)
        {
            var allElementsResponse = await _elementRepository.GetElementsQuery();

            var exists = allElementsResponse._Data?.Elements
                .Any(e => e.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)
                          && (excludeId == null || e.IdElement != excludeId));

            if (exists == true)
            {
                return ServiceResponse<ElementDTO>.Failure("Element o podanej nazwie już istnieje w systemie.", 409);
            }

            return ServiceResponse<ElementDTO>.Result(null!);
        }

        private bool IsDataUnchanged(ElementDTO current, UpdateElementRequest request)
        {
            return current.Name.Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete && current.ElementGroupId == request.ElementGroupId
                   && current.Description == request.Description;
        }

        #endregion
    }
}
