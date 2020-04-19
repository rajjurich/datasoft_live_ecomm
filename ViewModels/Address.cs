using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Address
    {
        public int AddressId { get; set; }
        [Required(ErrorMessage ="Address Required")]
        [StringLength(500)]
        public string FullAddress { get; set; }
        [StringLength(100)]
        public string City { get; set; }
        public int? Pincode { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
    }
}
