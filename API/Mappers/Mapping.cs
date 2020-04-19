using Domain.Entities;
using Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace API.Mappers
{
    public static class Mapping
    {
        public static CategoryDto ToCategoryDto(Category category)
        {
            var dto = new CategoryDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName
            };

            return dto;
        }
        public static Category ToCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                CategoryId = categoryDto.CategoryId,
                CategoryName = categoryDto.CategoryName
            };

            return category;
        }

        public static ManufacturerDto ToManufacturerDto(Manufacturer manufacturer)
        {
            var dto = new ManufacturerDto
            {
                ManufacturerId = manufacturer.ManufacturerId,
                ManufacturerName = manufacturer.ManufacturerName
            };

            return dto;
        }
        public static Manufacturer ToManufacturer(ManufacturerDto manufacturerDto)
        {
            var manufacturer = new Manufacturer
            {
                ManufacturerId = manufacturerDto.ManufacturerId,
                ManufacturerName = manufacturerDto.ManufacturerName
            };

            return manufacturer;
        }

        public static ProductTypeDto ToProductTypeDto(ProductType productType)
        {
            var dto = new ProductTypeDto
            {
                ProductTypeId = productType.ProductTypeId,
                ProductTypeName = productType.ProductTypeName
            };

            return dto;
        }
        public static ProductType ToProductType(ProductTypeDto productTypeDto)
        {
            var productType = new ProductType
            {
                ProductTypeId = productTypeDto.ProductTypeId,
                ProductTypeName = productTypeDto.ProductTypeName
            };

            return productType;
        }

        public static ResourceDto ToResourceDto(Resource resource)
        {
            var dto = new ResourceDto
            {
                ResourceId = resource.ResourceId,
                ResourceName = resource.ResourceName,
                RoleId = resource.RoleId,
                CompanyId = Convert.ToInt32(resource.CompanyId)
            };

            return dto;
        }

        public static Resource ToResource(ResourceDto resourceDto)
        {
            var resource = new Resource
            {
                ResourceId = resourceDto.ResourceId,
                ResourceName = resourceDto.ResourceName,
                RoleId = resourceDto.RoleId,
                CompanyId = resourceDto.CompanyId
            };

            return resource;
        }

        public static ProductDto ToProductDto(Product product)
        {
            var dto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                WeightInKg = product.WeightInKg,
                Quantity = product.Quantity,
                ProductDescription = product.ProductDescription,
                CategoryId = product.CategoryId,
                ManufacturerId = product.ManufacturerId,
                ProductTypeId = product.ProductTypeId,
                CompanyId = Convert.ToInt32(product.CompanyId)
            };

            return dto;
        }

        public static Product ToProduct(Product product, ProductDto productDto)
        {
            product.ProductId = productDto.ProductId;
            product.ProductName = productDto.ProductName;
            product.WeightInKg = productDto.WeightInKg;
            product.Quantity = productDto.Quantity;
            product.ProductDescription = productDto.ProductDescription;
            product.CategoryId = productDto.CategoryId;
            product.ManufacturerId = productDto.ManufacturerId;
            product.ProductTypeId = productDto.ProductTypeId;
            product.CompanyId = productDto.CompanyId;

            return product;
        }





        private static readonly Expression<Func<ProductsSalesOrder, ProductsSalesOrderDto>> AsProductsSalesOrdersDto =
            x => new ProductsSalesOrderDto
            {
                CGST = x.CGST,
                OrderPrice = x.OrderPrice,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                SGST = x.SGST,
                SalesOrderId = x.SalesOrderId,
                Products = new ProductDto
                {
                    ProductId = x.Product.ProductId,
                    ProductName = x.Product.ProductName,
                    WeightInKg = x.Product.WeightInKg,
                    Quantity = x.Product.Quantity,
                    ProductDescription = x.Product.ProductDescription,
                    CategoryId = x.Product.CategoryId,
                    ManufacturerId = x.Product.ManufacturerId,
                    ProductTypeId = x.Product.ProductTypeId,
                    CompanyId = Convert.ToInt32(x.Product.CompanyId)
                }
            };

        private static readonly Expression<Func<ProductsPurchaseOrder, ProductsPurchaseOrderDto>> AsProductsPurchaseOrdersDto =
            x => new ProductsPurchaseOrderDto
            {
                CGST = Convert.ToDecimal(x.CGST),
                OrderPrice = x.OrderPrice,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                SGST = Convert.ToDecimal(x.SGST),
                PurchaseOrderId = x.PurchaseOrderId,
                Products = new ProductDto
                {
                    ProductId = x.Product.ProductId,
                    ProductName = x.Product.ProductName,
                    WeightInKg = x.Product.WeightInKg,
                    Quantity = x.Product.Quantity,
                    ProductDescription = x.Product.ProductDescription,
                    CategoryId = x.Product.CategoryId,
                    ManufacturerId = x.Product.ManufacturerId,
                    ProductTypeId = x.Product.ProductTypeId,
                    CompanyId = Convert.ToInt32(x.Product.CompanyId)
                }
            };


        public static SalesOrderDto ToSalesOrderDto(SalesOrder salesOrder)
        {
            //var products = salesOrder.ProductsSalesOrders.AsQueryable().Select(x => x.Product).Select(AsProductInfoDto);
            var productsSalesOrders = salesOrder.ProductsSalesOrders.AsQueryable().Select(AsProductsSalesOrdersDto).ToList();
            //productsSalesOrders
            //var products =
            var dto = new SalesOrderDto
            {
                CompanyId = salesOrder.CompanyId,
                CustomerId = salesOrder.CustomerId,
                IsPaid = salesOrder.IsPaid,
                NetTotal = salesOrder.NetTotal,
                ResourceId = salesOrder.ResourceId,
                SalesOrderId = salesOrder.SalesOrderId,
                InvoiceDate = salesOrder.CreatedDate.ToString("dd MMM yyyy"),
                ProductsSalesOrders = productsSalesOrders.ToList()
            };
            return dto;
        }

        public static PurchaseOrderDto ToPurchaseOrderDto(PurchaseOrder purchaseOrder)
        {
            //var products = salesOrder.ProductsSalesOrders.AsQueryable().Select(x => x.Product).Select(AsProductInfoDto);
            var productsPurchaseOrders = purchaseOrder.ProductsPurchaseOrders.AsQueryable().Select(AsProductsPurchaseOrdersDto).ToList();
            //productsSalesOrders
            //var products =
            var dto = new PurchaseOrderDto
            {
                CompanyId = purchaseOrder.CompanyId,
                VendorId = purchaseOrder.VendorId,
                IsPaid = purchaseOrder.IsPaid,
                NetTotal = purchaseOrder.NetTotal,
                ResourceId = purchaseOrder.ResourceId,
                PurchaseOrderId = purchaseOrder.PurchaseOrderId,
                PurchaseDate = purchaseOrder.CreatedDate.ToString("dd MMM yyyy"),
                ProductsPurchaseOrders = productsPurchaseOrders.ToList()
            };
            return dto;
        }
        public static ProductsSalesOrderDto ToProductsSalesOrderDto(ProductsSalesOrder productsSalesOrder)
        {
            var dto = new ProductsSalesOrderDto
            {
                ProductId = productsSalesOrder.ProductId,
                OrderPrice = productsSalesOrder.OrderPrice,
                CGST = productsSalesOrder.CGST,
                //Products = ToProductDto(productsSalesOrder.Product),
                Quantity = productsSalesOrder.Quantity,
                SGST = productsSalesOrder.SGST
            };
            return dto;
        }
        public static SalesOrder ToSalesOrder(SalesOrder salesOrder, SalesOrderDto salesOrderDto)
        {
            salesOrder.IsPaid = salesOrderDto.IsPaid;
            salesOrder.ResourceId = salesOrderDto.ResourceId;
            salesOrder.CompanyId = salesOrderDto.CompanyId;
            salesOrder.CustomerId = salesOrderDto.CustomerId;
            return salesOrder;
        }

        public static PurchaseOrder ToPurchaseOrder(PurchaseOrder purchaseOrder, PurchaseOrderDto purchaseOrderDto)
        {
            purchaseOrder.IsPaid = purchaseOrderDto.IsPaid;
            purchaseOrder.ResourceId = purchaseOrderDto.ResourceId;
            purchaseOrder.CompanyId = purchaseOrderDto.CompanyId;
            purchaseOrder.VendorId = purchaseOrderDto.VendorId;
            purchaseOrder.IsPaid = purchaseOrderDto.IsPaid;
            purchaseOrder.NetTotal = 0;
            return purchaseOrder;
        }

        private static readonly Expression<Func<Address, AddressDto>> AsAddressDto =
           x => new AddressDto
           {
               AddressId = x.AddressId,
               City = x.City,
               DistrictId = x.DistrictId,
               FullAddress = x.FullAddress,
               Pincode = x.Pincode,
               StateId = x.StateId
           };

        public static CustomerDto ToCustomerDto(Customer customer)
        {
            var addressDtos = customer.Addresses.AsQueryable().Select(AsAddressDto).ToList();
            var dto = new CustomerDto
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                PrimaryMobileNumber = customer.PrimaryMobileNumber,
                EmailId = customer.EmailId,
                CompanyId = Convert.ToInt32(customer.CompanyId),
                GSTNumber = customer.GSTNumber,
                PAN = customer.GSTNumber,
                SecondaryMobileNumber = customer.SecondaryMobileNumber,
                Addresses = addressDtos
            };

            return dto;
        }

        public static Customer ToCustomer(Customer customer, CustomerDto customerDto)
        {
            customer.CustomerName = customerDto.CustomerName;
            customer.PrimaryMobileNumber = customerDto.PrimaryMobileNumber;
            customer.EmailId = customerDto.EmailId;
            customer.SecondaryMobileNumber = customerDto.SecondaryMobileNumber;
            customer.GSTNumber = customerDto.GSTNumber;
            customer.PAN = customerDto.PAN;
            customer.CompanyId = customerDto.CompanyId;
            return customer;
        }

        public static Address ToAddress(Address address,AddressDto addressDto)
        {
            //address.AddressId = addressDto.AddressId;
            address.FullAddress = addressDto.FullAddress;
            address.City = addressDto.City;
            address.DistrictId = addressDto.DistrictId;
            address.StateId = addressDto.StateId;           
            return address;
        }

        public static VendorDto ToVendorDto(Vendor vendor)
        {
            var dto = new VendorDto
            {
                VendorId = vendor.VendorId,
                VendorName = vendor.VendorName,
                PrimaryMobileNumber = vendor.PrimaryMobileNumber,
                EmailId = vendor.EmailId,
                CompanyId = Convert.ToInt32(vendor.CompanyId),
                GSTNumber = vendor.GSTNumber,
                PAN = vendor.GSTNumber,
                SecondaryMobileNumber = vendor.SecondaryMobileNumber
            };

            return dto;
        }

        public static Vendor ToVendor(Vendor vendor, VendorDto vendorDto)
        {
            vendor.VendorName = vendorDto.VendorName;
            vendor.PrimaryMobileNumber = vendorDto.PrimaryMobileNumber;
            vendor.EmailId = vendorDto.EmailId;
            vendor.SecondaryMobileNumber = vendorDto.SecondaryMobileNumber;
            vendor.GSTNumber = vendorDto.GSTNumber;
            vendor.PAN = vendorDto.PAN;
            vendor.CompanyId = vendorDto.CompanyId;
            return vendor;
        }

        public static CompanyDto ToCompanyDto(Company company)
        {
            var dto = new CompanyDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                PrimaryMobileNumber = company.PrimaryMobileNumber,
                EmailId = company.EmailId,
                GSTNumber = company.GSTNumber,
                PAN = company.GSTNumber,
                SecondaryMobileNumber = company.SecondaryMobileNumber
            };

            return dto;
        }

        public static Company ToCompany(Company company, CompanyDto companyDto)
        {
            company.CompanyName = companyDto.CompanyName;
            company.PrimaryMobileNumber = companyDto.PrimaryMobileNumber;
            company.EmailId = companyDto.EmailId;
            company.SecondaryMobileNumber = companyDto.SecondaryMobileNumber;
            company.GSTNumber = companyDto.GSTNumber;
            company.PAN = companyDto.PAN;
            return company;
        }
    }
}