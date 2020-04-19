using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string CategoryName { get; set; }
    }
}
