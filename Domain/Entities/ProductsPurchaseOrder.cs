using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ProductsPurchaseOrder
    {
        [Key, Column(Order = 1)]
        public int PurchaseOrderId { get; set; }
        [Key, Column(Order = 2)]        
        public int ProductId { get; set; }
        [Range(1,int.MaxValue)]
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal OrderPrice { get; set; }        
        public decimal? CGST { get; set; }
        public decimal? SGST { get; set; }       
        public PurchaseOrder PurchaseOrder { get; set; }        
        public Product Product { get; set; }
    }
}
