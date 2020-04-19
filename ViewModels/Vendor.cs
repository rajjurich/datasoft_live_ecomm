using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Vendor
    {
        public int VendorId { get; set; }
        [Display(Name = "Vendor Name")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string VendorName { get; set; }
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailId { get; set; }
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "required")]
        [RegularExpression(@"[\d]{10}", ErrorMessage = "Enter 10 digit Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter 10 digit Mobile Number")]
        public string PrimaryMobileNumber { get; set; }
        [Display(Name = "Alternate Number")]
        [RegularExpression(@"[\d]{10}", ErrorMessage = "Enter 10 digit Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter 10 digit Mobile Number")]
        public string SecondaryMobileNumber { get; set; }
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter valid Pancard")]
        [RegularExpression(@"[A-Z]{5}[0-9]{4}[A-Z]{1}", ErrorMessage = "Enter valid Pancard")]
        public string PAN { get; set; }
        [Display(Name = "GST")]
        public string GSTNumber { get; set; }
        [Display(Name = "Company")]
        public int CompanyId { get; set; }
    }
}
