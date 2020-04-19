using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Domain.Core;
using Domain.Entities;
using Domain.Services;
using System.Linq.Expressions;
using Dtos;
using API.Mappers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    [Authorize]
    public class ProductsController : ApiController
    {
        private readonly IProductService _productService;
        private readonly IResourceService _resourceService;

        private static readonly Expression<Func<Product, ProductInfoDto>> AsProductInfoDto =
            x => new ProductInfoDto
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                WeightInKg = x.WeightInKg,
                Quantity = x.Quantity,
                ProductDescription = x.ProductDescription,
                CategoryName = x.Category.CategoryName,
                ManufacturerName = x.Manufacturer.ManufacturerName,
                ProductTypeName = x.ProductType.ProductTypeName,                
                CompanyName = x.Company.CompanyName
            };
        public ProductsController(IProductService productService
            , IResourceService resourceService)
        {
            _productService = productService;
            _resourceService = resourceService;
        }

        // GET: api/Products
        public IQueryable<ProductInfoDto> GetProducts()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _productService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsProductInfoDto);
            }
            return _productService.GetAll().Select(AsProductInfoDto);
        }

        //// GET: api/GetProductNameListForPurchase        
        //[ResponseType(typeof(string))]
        //[Route("api/Products/GetProductNameListForPurchase")]
        //public IQueryable<string> GetProductNameListForPurchase()
        //{
        //    var productNameList = _productService.GetAll().Where(m => m.IsDeleted == false).Select(m => m.ProductName);

        //    return productNameList;
        //}

        //// GET: api/GetProductNameListForSales       
        //[ResponseType(typeof(string))]
        //[Route("api/Products/GetProductNameListForSales")]
        //public IQueryable<string> GetProductNameListForSales()
        //{
        //    var productNameList = _productService.GetAll().Where(m => m.IsDeleted == false && m.Quantity > 0).Select(m => m.ProductName);

        //    return productNameList;
        //}

        // GET: api/Products/5
        [ResponseType(typeof(ProductDto))]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await _productService.GetSingle(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToProductDto(product));
        }

        // PUT: api/Products/5
        [ResponseType(typeof(ProductDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutProduct(int id, ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productDto.ProductId)
            {
                return BadRequest("Mismatch");
            }
            var product = await _productService.GetSingle(id);

            if (product == null)
            {
                return NotFound();
            }
            var updatedProduct = await _productService.Edit(Mapping.ToProduct(product, productDto));

            return Ok(Mapping.ToProductDto(updatedProduct));

        }

        // POST: api/Products
        [ResponseType(typeof(ProductDto))]
        public async Task<IHttpActionResult> PostProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = new Product();
            productDto.Quantity = 0;
            var product = await _productService.Add(Mapping.ToProduct(newProduct, productDto));

            return CreatedAtRoute("DefaultApi", new { id = product.ProductId }, Mapping.ToProductDto(product));
        }

        // DELETE: api/Products/5
        [ResponseType(typeof(ProductDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await _productService.GetSingle(id);

            Product deletedProduct = await _productService.Delete(product);

            return Ok(Mapping.ToProductDto(deletedProduct));
        }

    }
}