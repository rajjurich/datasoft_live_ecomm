using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Manufacturer
    {
        public int ManufacturerId { get; set; }
        [Display(Name = "Manufacturer")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string ManufacturerName { get; set; }
    }
}
