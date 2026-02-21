using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Models.Client.Requests
{
    public class ClientRequest
    {
        public int IdClient { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public StatusClient? StatusClient { get; set; }
    }
}
