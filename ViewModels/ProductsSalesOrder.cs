﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    public class ProductsSalesOrder
    {
        [Key]
        public int ProductsSalesOrderId { get; set; }
        public int? SalesOrderId { get; set; }
        public string ProductName { get; set; }
        public decimal? WeightInKg { get; set; }
        public string ManufacturerName { get; set; }
        public string ProductTypeName { get; set; }
        public string CategoryName { get; set; }
        public int ProductId { get; set; }
        [Required(ErrorMessage = "required")]
        public int? Quantity { get; set; }
        [Column(TypeName = "money")]
        [Required(ErrorMessage = "required")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid")]
        [Display(Name = "Price")]
        public decimal OrderPrice { get; set; }
        [Required(ErrorMessage = "required")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid")]
        public decimal CGST { get; set; }
        [Required(ErrorMessage = "required")]
        [DataType(DataType.Currency, ErrorMessage = "Invalid")]
        public decimal SGST { get; set; }
        public string CgstPercent { get; set; }
        public string SgstPercent { get; set; }
        [DataType(DataType.Currency, ErrorMessage = "Invalid")]
        public decimal RowTotal { get; set; }
        public string ProductDescription { get; set; }
    }
}
