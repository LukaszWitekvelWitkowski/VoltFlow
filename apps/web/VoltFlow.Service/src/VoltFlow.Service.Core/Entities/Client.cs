namespace VoltFlow.Service.Core.Entities
{
    public class Client
    {
        public int IdClient { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int TenantId { get; set; }

        // Relacje
        public ICollection<ClientAddress> Addresses { get; set; } = new List<ClientAddress>();
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
