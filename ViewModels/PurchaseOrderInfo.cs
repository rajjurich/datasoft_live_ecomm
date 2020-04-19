using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class PurchaseOrderInfo
    {
        [Display(Name = "Order Id")]
        public int PurchaseOrderId { get; set; }
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal NetTotal { get; set; }
        [Display(Name = "Payment Status")]
        public string IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public int ResourceId { get; set; }
        [Display(Name = "Resource Name")]
        public string ResourceName { get; set; }
        public int VendorId { get; set; }
        [Display(Name = "Customer Name")]
        public string VendorName { get; set; }
        [Display(Name = "Mobile Number")]
        public long PrimaryMobileNumber { get; set; }
        [Display(Name = "Invoice Date")]
        public string InvoiceDate { get; set; }
        public List<ProductsPurchaseOrder> productsPurchaseOrder { get; set; }
    }
}
