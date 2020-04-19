using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class PurchaseOrder
    {
        public int PurchaseOrderId { get; set; }
        [Column(TypeName = "money")]
        public decimal NetTotal { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int ResourceId { get; set; }
        public int VendorId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public Byte[] Timestamp { get; set; }
        public ICollection<ProductsPurchaseOrder> ProductsPurchaseOrders { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [ForeignKey("ResourceId")]
        public Resource Resource { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
    }
}
