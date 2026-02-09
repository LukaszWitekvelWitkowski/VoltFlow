using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Models.ElementGroup.Request
{
    public class UpdateElementGroupRequest
    {
        public int IdElementGroup { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsObsolete { get; set; }
        public int CategoryId { get; set; }
    }
}
