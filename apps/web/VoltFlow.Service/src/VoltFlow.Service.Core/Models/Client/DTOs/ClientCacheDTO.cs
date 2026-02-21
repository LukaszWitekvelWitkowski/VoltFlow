using VoltFlow.Service.Core.Abstractions.Generic;

namespace VoltFlow.Service.Core.Models.Client.DTOs
{
    public class ClientCacheDTO : ICacheData<ClientDTO>
    {
        public IEnumerable<ClientDTO> Items { get; set; } = new List<ClientDTO>();

        public void insert(IEnumerable<ClientDTO> items)
        {
            Items = items;
        }
    }
}
