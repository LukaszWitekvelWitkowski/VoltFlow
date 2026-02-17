using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class EmailLog
    {
        public int Id { get; set; }
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public EmailTypeEnum EmailType { get; set; }
        public int? RelatedClientId { get; set; }
    }
}
