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

namespace API.Controllers
{
    [Authorize]
    public class ProductTypesController : ApiController
    {
        private readonly IProductTypeService _productTypeService;

        private static readonly Expression<Func<ProductType, ProductTypeDto>> AsProductTypeDto =
            x => new ProductTypeDto
            {
                ProductTypeId = x.ProductTypeId,
                ProductTypeName = x.ProductTypeName
            };
        public ProductTypesController(IProductTypeService productTypeService)
        {
            _productTypeService = productTypeService;
        }

        // GET: api/ProductTypes
        public IQueryable<ProductTypeDto> GetProductTypes()
        {
            return _productTypeService.GetAll().Select(AsProductTypeDto);
        }

        // GET: api/ProductTypes/5
        [ResponseType(typeof(ProductTypeDto))]
        public async Task<IHttpActionResult> GetProductType(int id)
        {
            ProductType productType = await _productTypeService.GetSingle(id);
            if (productType == null)
            {
                return NotFound();
            }

            return Ok(Mapping.ToProductTypeDto(productType));
        }

        // PUT: api/ProductTypes/5
        [ResponseType(typeof(ProductTypeDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutProductType(int id, ProductTypeDto productTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productTypeDto.ProductTypeId)
            {
                return BadRequest("Mismatch");
            }

            var updatedProductType = await _productTypeService.Edit(Mapping.ToProductType(productTypeDto));

            return Ok(Mapping.ToProductTypeDto(updatedProductType));
        }

        // POST: api/ProductTypes
        [ResponseType(typeof(ProductTypeDto))]
        public async Task<IHttpActionResult> PostProductType(ProductTypeDto productTypeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productType = await _productTypeService.Add(Mapping.ToProductType(productTypeDto));

            return CreatedAtRoute("DefaultApi", new { id = productType.ProductTypeId }, Mapping.ToProductTypeDto(productType));            
        }

        // DELETE: api/ProductTypes/5
        [ResponseType(typeof(ProductTypeDto))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            ProductType productType = await _productTypeService.GetSingle(id);

            ProductType deletedProductType = await _productTypeService.Delete(productType);

            return Ok(Mapping.ToProductTypeDto(deletedProductType));            
        }
        
    }
}