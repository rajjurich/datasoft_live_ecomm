using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class VendorDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string EmailId { get; set; }
        public long PrimaryMobileNumber { get; set; }
        public long? SecondaryMobileNumber { get; set; }
        public string PAN { get; set; }
        public string GSTNumber { get; set; }
        public int CompanyId { get; set; }
    }
}
