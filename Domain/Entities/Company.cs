using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Company
    {
        public int CompanyId { get; set; }
        [Required]
        [Index(IsUnique = true)]
        [StringLength(100)]
        public string CompanyName { get; set; }
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
        [ConcurrencyCheck]
        [Timestamp]
        public Byte[] TimeStamp { get; set; }
        public ICollection<Address> Addresses { get; set; }
        public ICollection<SalesOrder> SalesOrders { get; set; }
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public ICollection<Resource> Resources { get; set; }
    }
}
