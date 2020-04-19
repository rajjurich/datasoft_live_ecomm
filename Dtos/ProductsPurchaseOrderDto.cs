using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class ProductsPurchaseOrderDto
    {
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public ProductDto Products { get; set; }
    }
}
