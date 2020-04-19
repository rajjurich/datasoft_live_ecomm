using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ProductInfo
    {
        public int ProductId { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Display(Name = "Weight In Kg")]
        public decimal? WeightInKg { get; set; }
        [Display(Name = "Quantity")]
        public int? Quantity { get; set; }
        [Display(Name = "Product Description")]        
        public string ProductDescription { get; set; }        
        [Display(Name = "Product Type")]
        public string ProductTypeName { get; set; }
        [Display(Name = "Manufacturer")]
        public string ManufacturerName { get; set; }
        [Display(Name = "Category")]
        public string CategoryName { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; }
    }
}
