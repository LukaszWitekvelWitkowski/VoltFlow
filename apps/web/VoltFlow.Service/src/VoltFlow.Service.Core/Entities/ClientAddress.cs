using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class ClientAddress
    {
        public int IdAddress { get; set; }
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string NumberStreet { get; set; } = string.Empty;
        public AddressType AddressType { get; set; } 
        public string LocationNumber { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }

        // Relacje
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
