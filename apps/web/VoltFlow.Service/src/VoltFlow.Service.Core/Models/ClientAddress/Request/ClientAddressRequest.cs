using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Models.ClientAddress.Request
{
    public class ClientAddressRequest
    {
        public int IdAddress { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string NumberStreet { get; set; } = string.Empty;
        public AddressType AddressType { get; set; }
        public string LocationNumber { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
        public int ClientId { get; set; }
    }
}
