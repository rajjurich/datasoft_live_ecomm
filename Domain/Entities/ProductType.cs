﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProductType
    {
        public int ProductTypeId { get; set; }
        [Required]
        [Index("IX_ProductTypeNameAndIsDeleted", 1, IsUnique = true)]        
        [StringLength(100)]
        public string ProductTypeName { get; set; }
        [Index("IX_ProductTypeNameAndIsDeleted", 2, IsUnique = true)]
        public bool IsDeleted { get; set; }
        public ICollection<Product> Products { get; set; }
    }

}
