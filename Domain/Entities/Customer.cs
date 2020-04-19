using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        [Required]
        [Index("IX_CustomerNameAndCompanyID", 1, IsUnique = true)]        
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [Index("IX_CustomerNameAndCompanyID", 2, IsUnique = true)]
        public int? CompanyId { get; set; }
        [StringLength(100)]
        public string EmailId { get; set; }
        [Index(IsUnique = true)]
        public long PrimaryMobileNumber { get; set; }
        public long? SecondaryMobileNumber { get; set; }
        [StringLength(10)]
        public string PAN { get; set; }
        [StringLength(100)]
        public string GSTNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public Byte[] Timestamp { get; set; }
        public ICollection<SalesOrder> SalesOrders { get; set; }
        public ICollection<Address> Addresses { get; set; }        
    }
}
