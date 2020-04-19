using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Resource
    {
        public int ResourceId { get; set; }
        [Required]
        [Index("IX_ResourceNameAndCompanyID", 1, IsUnique = true)]        
        [StringLength(100)]
        public string ResourceName { get; set; }        
        [Index("IX_ResourceNameAndCompanyID", 2, IsUnique = true)]
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<SalesOrder> SalesOrders { get; set; }
    }
}
