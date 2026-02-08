namespace VoltFlow.Service.Core.Models.ElementGroup.DTOs
{
    public class ElementGroupsDTO
    {
        public IEnumerable<ElementGroupDTO> ElementGroups { get; set; }

        public ElementGroupsDTO(IEnumerable<ElementGroupDTO>? elementGroups)
        {
            if (elementGroups == null)
            {
                ElementGroups = new List<ElementGroupDTO>();
                return;
            }
            ElementGroups = elementGroups;
        }
    }
}
