using System.ComponentModel.DataAnnotations;
using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class Doc
    {
        [Key]
        public int IdDocs { get; set; }

        public int ClientId { get; set; }

        public DateTime CreateTime { get; set; }

        // Zakładam, że masz już zdefiniowany enum StatusDocs
        public StatusDoc StatusDoc { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalVat { get; set; }

        public decimal TotalGross { get; set; }

        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<DocElement> DocElements { get; set; } = new List<DocElement>();
    }
}