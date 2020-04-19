using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class Product
    {
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string ProductName { get; set; }
        [Display(Name = "Weight In Kg")]
        //[Required(ErrorMessage = "required")]         
        public decimal? WeightInKg { get; set; }
        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }
        [Display(Name = "Product Description")]        
        [StringLength(500, ErrorMessage = "Maximum length allowed is 500")]
        public string ProductDescription { get; set; }        
        [Display(Name = "Product Type")]
        [Required(ErrorMessage = "required")]
        public int ProductTypeId { get; set; }
        [Display(Name = "Manufacturer")]
        [Required(ErrorMessage = "required")]
        public int ManufacturerId { get; set; }
        [Display(Name = "Category")]
        [Required(ErrorMessage = "required")]
        public int CategoryId { get; set; }
        [Display(Name = "Company")]
        [Required(ErrorMessage = "required")]
        public int CompanyId { get; set; }
    }
}
