using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ProductsSalesOrder
    {
        [Key, Column(Order = 1)]
        public int SalesOrderId { get; set; }
        [Key, Column(Order = 2)]       
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal OrderPrice { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }        
        public SalesOrder SalesOrder { get; set; }        
        public Product Product { get; set; }
    }
}
