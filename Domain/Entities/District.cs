using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class District
    {
        public int DistrictId { get; set; }
        [Required]
        [Index(IsUnique =true)]
        [StringLength(100)]
        public string DistrictName { get; set; }
        public int? StateId { get; set; }
        [ForeignKey("StateId")]
        public State State { get; set; }
        public ICollection<Address> Addresses { get; set; }
    }
}
