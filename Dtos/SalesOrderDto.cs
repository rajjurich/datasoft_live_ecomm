using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dtos
{
    public class SalesOrderDto
    {
        public int SalesOrderId { get; set; }        
        public decimal NetTotal { get; set; }
        public bool IsPaid { get; set; }
        public bool IsDeleted { get; set; }       
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public int ResourceId { get; set; }        
        public string ResourceName { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long PrimaryMobileNumber { get; set; }
        public string InvoiceDate { get; set; }
        public ICollection<ProductsSalesOrderDto> ProductsSalesOrders { get; set; }        
    }
}
