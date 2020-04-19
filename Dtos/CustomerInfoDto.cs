using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class CustomerInfoDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }                
        public string EmailId { get; set; }
        public long PrimaryMobileNumber { get; set; }        
        public long? SecondaryMobileNumber { get; set; }       
        public string PAN { get; set; }        
        public string GSTNumber { get; set; }
        public string CompanyName { get; set; }
    }
}
