using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class Client
    {
        public int IdClient { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public StatusClient statusClient { get; set; } = StatusClient.Active;

        // Relacje
        public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
