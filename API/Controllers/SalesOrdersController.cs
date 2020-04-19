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
using Dtos;
using Domain.Services;
using System.Linq.Expressions;
using API.Mappers;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace API.Controllers
{
    [Authorize]
    public class SalesOrdersController : ApiController
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly IProductsSalesOrderService _productsSalesOrderService;
        private readonly IProductService _productService;
        private readonly IResourceService _resourceService;

        private static readonly Expression<Func<SalesOrder, SalesOrderInfoDto>> AsSalesOrderDto =
            x => new SalesOrderInfoDto
            {
                CompanyId = x.CompanyId,
                CustomerId = x.CustomerId,
                NetTotal = x.NetTotal,
                IsPaid = x.IsPaid ? "Paid" : "Unpaid",
                ResourceId = x.ResourceId,
                SalesOrderId = x.SalesOrderId,
                CustomerName = x.Customer.CustomerName,
                CompanyName = x.Company.CompanyName,
                ResourceName = x.Resource.ResourceName,
                PrimaryMobileNumber = x.Customer.PrimaryMobileNumber,
            };
        public SalesOrdersController(ISalesOrderService salesOrderService
            , IProductsSalesOrderService productsSalesOrderService
            , IProductService productService
            , IResourceService resourceService)
        {
            _salesOrderService = salesOrderService;
            _productsSalesOrderService = productsSalesOrderService;
            _productService = productService;
            _resourceService = resourceService;
        }

        // GET: api/SalesOrders
        //[AllowAnonymous]
        public IQueryable<SalesOrderInfoDto> GetSalesOrders()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _salesOrderService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsSalesOrderDto);
            }            
            return _salesOrderService.GetAll().Select(AsSalesOrderDto);
        }

        // GET: api/SalesOrders/5
        [ResponseType(typeof(SalesOrderDto))]
        public async Task<IHttpActionResult> GetSalesOrder(int id)
        {
            SalesOrder salesOrder = await _salesOrderService.GetSingle(id);
            salesOrder.ProductsSalesOrders = _productsSalesOrderService.GetAll().Include(x => x.Product).Where(x => x.SalesOrderId == id).ToList();
            if (salesOrder == null)
            {
                return NotFound();
            }
            //Mapper.Configuration.CreateMapper<SalesOrderDto, SalesOrder>();
            //Mapper.Map<SalesOrderDto>(salesOrder);
            return Ok(Mapping.ToSalesOrderDto(salesOrder));
        }

        // PUT: api/SalesOrders/5
        [ResponseType(typeof(SalesOrder))]
        public async Task<IHttpActionResult> PutSalesOrder(int id, SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var getSalesOrder = await _salesOrderService.GetSingle(id);

            if (getSalesOrder == null)
            {
                return NotFound();
            }
            getSalesOrder.IsPaid = true;
            var updatedVendor = await _salesOrderService.Edit(getSalesOrder);

            return Ok(updatedVendor);
        }
        // PUT: api/SalesOrders/5
        [ResponseType(typeof(SalesOrder))]
        [Route("api/SalesOrders/Pay/{id}")]
        public async Task<IHttpActionResult> PutSalesOrderAsPaid(int id, SalesOrder salesOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesOrder.SalesOrderId)
            {
                return BadRequest("Mismatch");
            }
            var _salesOrder = await _salesOrderService.GetSingle(id);

            if (_salesOrder == null)
            {
                return NotFound();
            }
            _salesOrder.IsPaid = true;
            var updatedSalesOrder = await _salesOrderService.Edit(_salesOrder);

            return Ok(updatedSalesOrder);
        }
        // POST: api/SalesOrders
        [ResponseType(typeof(SalesOrderDto))]
        public async Task<IHttpActionResult> PostSalesOrder(SalesOrderDto salesOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newSalesOrder = new SalesOrder();
            var salesOrder = await _salesOrderService.Add(Mapping.ToSalesOrder(newSalesOrder, salesOrderDto));
            decimal netTotal = 0;

            foreach (var item in salesOrderDto.ProductsSalesOrders)
            {
                var productsSaleOrder = new ProductsSalesOrder();
                productsSaleOrder.SalesOrderId = salesOrder.SalesOrderId;
                productsSaleOrder.ProductId = item.ProductId;
                productsSaleOrder.OrderPrice = item.OrderPrice;
                productsSaleOrder.CGST = item.CGST;
                productsSaleOrder.SGST = item.SGST;
                productsSaleOrder.Quantity = item.Quantity;
                var amt = CalculateAmount(productsSaleOrder);
                netTotal += amt;
                var productsSaleOrderCreated = await _productsSalesOrderService.Add(productsSaleOrder);

                Product product = await _productService.GetSingle(item.ProductId);
                var quantity = product.Quantity - item.Quantity;
                if (quantity < 0)
                {
                    throw new Exception(string.Format("{0} is not available in {1} nos", product.ProductName, item.Quantity));
                }
                product.Quantity = quantity;
                var updatedProduct = await _productService.Edit(product);
            }
            salesOrder.NetTotal = netTotal;
            salesOrder = await _salesOrderService.Edit(salesOrder);
            return CreatedAtRoute("DefaultApi", new { id = salesOrder.SalesOrderId }, Mapping.ToSalesOrderDto(salesOrder));

        }

        // DELETE: api/SalesOrders/5
        //[ResponseType(typeof(SalesOrder))]
        //public async Task<IHttpActionResult> DeleteSalesOrder(int id)
        //{
        //    SalesOrder salesOrder = await db.SalesOrders.FindAsync(id);
        //    if (salesOrder == null)
        //    {
        //        return NotFound();
        //    }

        //    db.SalesOrders.Remove(salesOrder);
        //    await db.SaveChangesAsync();

        //    return Ok(salesOrder);
        //}        

        private decimal CalculateAmount(ProductsSalesOrder productsSalesOrder)
        {
            var price = productsSalesOrder.OrderPrice;
            var cgst = productsSalesOrder.CGST;
            var sgst = productsSalesOrder.SGST;
            var amt = price + (price * (cgst / 100)) + (price * (sgst / 100));
            return amt * productsSalesOrder.Quantity;
        }
    }
}