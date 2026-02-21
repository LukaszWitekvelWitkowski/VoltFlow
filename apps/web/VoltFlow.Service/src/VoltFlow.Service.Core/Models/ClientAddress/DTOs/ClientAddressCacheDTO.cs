using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.ClientAddress.DTOs
{
    public class ClientAddressCacheDTO : ICacheData<ClientAddressDTO>
    {
        public IEnumerable<ClientAddressDTO> Items { get; set; } = new List<ClientAddressDTO>();
        public void insert(IEnumerable<ClientAddressDTO> items)
        {
            Items = items;
        }
    }
}
