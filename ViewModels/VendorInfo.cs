using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class VendorInfo
    {
        public int VendorId { get; set; }
        [Display(Name = "Vendor Name")]
        public string VendorName { get; set; }
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
    }
}
