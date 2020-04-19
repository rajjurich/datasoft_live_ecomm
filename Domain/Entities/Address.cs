using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Address
    {
        public int AddressId { get; set; }
        [Required]
        [StringLength(500)]
        public string FullAddress { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        public int? Pincode { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        [ForeignKey("DistrictId")]
        public District District { get; set; }
        [ForeignKey("StateId")]
        public State State { get; set; }
        public int? CompanyId { get; set; }
        public int? CustomerId { get; set; }
        public int? VendorId { get; set; }
        public Company Company { get; set; }
        public Customer Customer { get; set; }
        public Vendor Vendor { get; set; }
    }
}
