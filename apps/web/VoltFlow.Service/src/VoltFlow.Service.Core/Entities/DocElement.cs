using System.ComponentModel.DataAnnotations;

namespace VoltFlow.Service.Core.Entities
{
    public class DocElement
    {
        [Key]
        public int IdDocsElement { get; set; }

        public int IdDoc { get; set; }

        public virtual Doc Doc { get; set; } = null!;

        public int TransactionPostId { get; set; }

        public int ElementId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPriceNet { get; set; }

        public decimal UnitPriceGross { get; set; }

        public decimal VatRate { get; set; }
    }
}
