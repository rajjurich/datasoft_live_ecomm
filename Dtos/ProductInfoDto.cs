using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class ProductInfoDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? WeightInKg { get; set; }
        public int? Quantity { get; set; }
        public string ProductDescription { get; set; }        
        public string CategoryName { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductTypeName { get; set; }
        public string CompanyName { get; set; }
    }
}
