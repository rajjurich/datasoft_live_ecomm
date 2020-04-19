using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class AddressDto
    {
        public int AddressId { get; set; }        
        public string FullAddress { get; set; }        
        public string City { get; set; }
        public int? Pincode { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
    }
}
