using VoltFlow.Service.Core.Abstractions.Repositories;
using VoltFlow.Service.Core.Abstractions.Services;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;
using VoltFlow.Service.Core.Models.Element.DTOs;
using VoltFlow.Service.Core.Models.Element.Request;

namespace VoltFlow.Service.Application.Services
{

public class ElementService : IElementService
    {
        private readonly IElementRepository _elementRepository;

        public ElementService(IElementRepository elementRepository) => _elementRepository = elementRepository;

        public async Task<ServiceResponse<ElementDTO>> CreateElement(CreateElementRequest request)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa elementu nie może być pusta.");

            // 2. Quick duplicate check in the database
            if (await _elementRepository.IsExists(request.Name))
                throw new ConflictException("Element o podanej nazwie już istnieje w systemie.");

            // 3. Save
            var result = await _elementRepository.AddElement(request);
            return ServiceResponse<ElementDTO>.Success(result._Data!);
        }

        public async Task<ServiceResponse<ElementDTO>> UpdateElement(UpdateElementRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationEntityException("Nazwa elementu nie może być pusta.");

            // 1. Downloading data for comparison
            var currentResponse = await _elementRepository.GetElementByIdQuery(request.Id);
            var currentElement = currentResponse._Data ?? throw new NotFoundException("Element nie istnieje.");

            // 2. Checking if any change has occurred (Idempotence)
            if (IsDataUnchanged(currentElement, request))
                return ServiceResponse<ElementDTO>.Success(currentElement);

            // 3. Duplicate name check (excluding edited ID)
            if (await _elementRepository.IsExists(request.Name, request.Id))
                throw new ConflictException("Element o podanej nazwie już istnieje w systemie.");

            // 4. Update
            var updated = await _elementRepository.UpdateElement(request);
            return ServiceResponse<ElementDTO>.Success(updated._Data!);
        }

        #region Private Helper Methods

        private bool IsDataUnchanged(ElementDTO current, UpdateElementRequest request)
        { 
            return current.Name.Trim().Equals(request.Name.Trim(), StringComparison.OrdinalIgnoreCase)
                   && current.IsObsolete == request.IsObsolete
                   && current.ElementGroupId == request.ElementGroupId
                   && (current.Description ?? string.Empty) == (request.Description ?? string.Empty);
        }

        #endregion
    }
}
