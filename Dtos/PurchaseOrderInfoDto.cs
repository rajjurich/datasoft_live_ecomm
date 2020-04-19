using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class PurchaseOrderInfoDto
    {
        public int PurchaseOrderId { get; set; }        
        public decimal NetTotal { get; set; }
        public string IsPaid { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public long PrimaryMobileNumber { get; set; }        
    }
}
