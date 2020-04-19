using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        [Required]
        [Index("IX_ProductNameAndWeightAndCompanyId", 1, IsUnique = true)]
        [StringLength(100)]
        public string ProductName { get; set; }        
        [Index("IX_ProductNameAndWeightAndCompanyId", 2, IsUnique = true)]
        public decimal? WeightInKg { get; set; }
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }       
        [StringLength(500)]
        public string ProductDescription { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int ProductTypeId { get; set; }
        public int ManufacturerId { get; set; }
        public int CategoryId { get; set; }
        [Index("IX_ProductNameAndWeightAndCompanyId", 3, IsUnique = true)]
        public int? CompanyId { get; set; }
        [ConcurrencyCheck]
        [Timestamp]
        public Byte[] Timestamp { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }
        [ForeignKey("ProductTypeId")]
        public ProductType ProductType { get; set; }
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

    }
}
