using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class SalesOrder
    {
        [Key]
        public int SalesOrderId { get; set; }
        public int? CustomerId { get; set; }
        [Display(Name = "Customer Name")]
        [Required(ErrorMessage = "required")]
        [StringLength(100, ErrorMessage = "Maximum length allowed is 100")]
        public string CustomerName { get; set; }
        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "required")]
        [RegularExpression(@"[\d]{10}", ErrorMessage = "Enter 10 digit Mobile Number")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Enter 10 digit Mobile Number")]
        public string PrimaryMobileNumber { get; set; }
        public decimal? NetTotal { get; set; }
        public bool IsPaid { get; set; }
        [Display(Name ="Company")]
        [Required(ErrorMessage ="required")]
        public int CompanyId { get; set; }
        [Display(Name = "Resource")]
        [Required(ErrorMessage = "required")]
        public int ResourceId { get; set; }
        [Display(Name = "Products")]
        public int ProductId { get; set; }
        [Required]
        public List<ProductsSalesOrder> productsSalesOrders { get; set; }
    }
}
