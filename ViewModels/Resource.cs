using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Resource
    {
        public int ResourceId { get; set; }
        [Display(Name = "Resource")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string ResourceName { get; set; }
        [Display(Name = "Company")]
        [Required(ErrorMessage = "required")]
        public int CompanyId { get; set; }
        [Display(Name = "Role")]
        [Required(ErrorMessage = "required")]
        public int RoleId { get; set; }
        [Display(Name = "Email")]
        [Required(ErrorMessage = "required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password Mismatch")]
        public string ConfirmPassword { get; set; }
    }
}
