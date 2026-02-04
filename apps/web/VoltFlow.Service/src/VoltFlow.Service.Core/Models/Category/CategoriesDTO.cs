using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Models.Category
{
    public class CategoriesDTO
    {
        public required List<CategoryDTO> Categories { get; set; }
    }
}
