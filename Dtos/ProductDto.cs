using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? WeightInKg { get; set; }
        public int? Quantity { get; set; }
        public string ProductDescription { get; set; }        
        public int ProductTypeId { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
    }
}
