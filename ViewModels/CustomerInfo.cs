using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class CustomerInfo
    {
        public int CustomerId { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Display(Name = "Email Address")]
        public string EmailId { get; set; }
        [Display(Name = "Mobile Number")]
        public string PrimaryMobileNumber { get; set; }
        [Display(Name = "Alternate Number")]        
        public string SecondaryMobileNumber { get; set; }        
        public string PAN { get; set; }
        [Display(Name = "GST")]
        public string GSTNumber { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}
