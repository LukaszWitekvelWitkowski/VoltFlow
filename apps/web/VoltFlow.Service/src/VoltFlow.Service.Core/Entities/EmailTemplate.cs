using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class EmailTemplate
    {
        public int Id { get; set; }
        public EmailTypeEnum EmailType { get; set; } 
        public string Name { get; set; } = string.Empty; 
        public string Subject { get; set; } = string.Empty;
        public string BodyHtml { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
