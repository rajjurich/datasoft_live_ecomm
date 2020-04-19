using API.Mappers;
using Domain.Entities;
using Domain.Services;
using Dtos;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace API.Controllers
{
    [Authorize]
    public class PurchaseOrdersController : ApiController
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IProductsPurchaseOrderService _productsPurchaseOrderService;
        private readonly IProductService _productService;
        private readonly IResourceService _resourceService;

        private static readonly Expression<Func<PurchaseOrder, PurchaseOrderInfoDto>> AsPurchaseOrderDto =
            x => new PurchaseOrderInfoDto
            {
                CompanyId = x.CompanyId,
                VendorId = x.VendorId,
                NetTotal = x.NetTotal,
                IsPaid = x.IsPaid ? "Paid" : "Unpaid",
                ResourceId = x.ResourceId,
                PurchaseOrderId = x.PurchaseOrderId,
                VendorName = x.Vendor.VendorName,
                CompanyName = x.Company.CompanyName,
                ResourceName = x.Resource.ResourceName,
                PrimaryMobileNumber = x.Vendor.PrimaryMobileNumber,

            };
        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService
            , IProductsPurchaseOrderService productsPurchaseOrderService
            , IProductService productService
            , IResourceService resourceService)
        {
            _purchaseOrderService = purchaseOrderService;
            _productsPurchaseOrderService = productsPurchaseOrderService;
            _productService = productService;
            _resourceService = resourceService;
        }

        // GET: api/PurchaseOrders
        //[AllowAnonymous]
        public IQueryable<PurchaseOrderInfoDto> GetPurchaseOrders()
        {
            var userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roles = userManager.GetRolesAsync(User.Identity.GetUserId()).Result;
            var user = User.Identity.Name;
            if (roles[0] == "user")
            {
                var companies = _resourceService.GetAll().Where(x => x.ResourceName == user).Select(x => x.CompanyId).ToList();
                return _purchaseOrderService.GetAll().Where(x => companies.Contains(x.CompanyId)).Select(AsPurchaseOrderDto);
            }           
            return _purchaseOrderService.GetAll().Select(AsPurchaseOrderDto);
        }

        // GET: api/PurchaseOrders/5
        [ResponseType(typeof(PurchaseOrderDto))]
        public async Task<IHttpActionResult> GetPurchaseOrder(int id)
        {
            PurchaseOrder purchaseOrder = await _purchaseOrderService.GetSingle(id);
            purchaseOrder.ProductsPurchaseOrders = _productsPurchaseOrderService.GetAll().Include(x => x.Product).Where(x => x.PurchaseOrderId == id).ToList();
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            //Mapper.Configuration.CreateMapper<SalesOrderDto, SalesOrder>();
            //Mapper.Map<SalesOrderDto>(salesOrder);
            return Ok(Mapping.ToPurchaseOrderDto(purchaseOrder));
        }

        // PUT: api/PurchaseOrders/5
        [ResponseType(typeof(PurchaseOrder))]
        [Authorize(Roles = "superadmin,admin")]
        public async Task<IHttpActionResult> PutPurchaseOrder(int id, PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var getPurchaseOrder = await _purchaseOrderService.GetSingle(id);

            if (getPurchaseOrder == null)
            {
                return NotFound();
            }
            getPurchaseOrder.IsPaid = true;
            var updatedSalesOrder = await _purchaseOrderService.Edit(getPurchaseOrder);

            return Ok(updatedSalesOrder);
        }
        // PUT: api/PurchaseOrders/5
        [ResponseType(typeof(PurchaseOrder))]
        [Route("api/PurchaseOrders/Pay/{id}")]
        public async Task<IHttpActionResult> PutPurchaseOrderAsPaid(int id, PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrder.PurchaseOrderId)
            {
                return BadRequest("Mismatch");
            }
            var _purchaseOrder = await _purchaseOrderService.GetSingle(id);

            if (_purchaseOrder == null)
            {
                return NotFound();
            }
            _purchaseOrder.IsPaid = true;
            var updatedPurchaseOrder = await _purchaseOrderService.Edit(_purchaseOrder);

            return Ok(updatedPurchaseOrder);
        }
        // POST: api/PurchaseOrders
        [ResponseType(typeof(PurchaseOrderDto))]
        public async Task<IHttpActionResult> PostSalesOrder(PurchaseOrderDto purchaseOrderDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var _purchaseOrder = new PurchaseOrder();
            var purchaseOrder = await _purchaseOrderService.Add(Mapping.ToPurchaseOrder(_purchaseOrder, purchaseOrderDto));
            decimal netTotal = 0;

            foreach (var item in purchaseOrderDto.ProductsPurchaseOrders)
            {
                var productsPurchaseOrder = new ProductsPurchaseOrder();
                productsPurchaseOrder.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                productsPurchaseOrder.ProductId = item.ProductId;
                productsPurchaseOrder.OrderPrice = item.OrderPrice;
                productsPurchaseOrder.CGST = 0;
                productsPurchaseOrder.SGST = 0;
                productsPurchaseOrder.Quantity = item.Quantity;
                var amt = CalculateAmount(productsPurchaseOrder);
                netTotal += amt;
                var productsSaleOrderCreated = await _productsPurchaseOrderService.Add(productsPurchaseOrder);

                Product product = await _productService.GetSingle(item.ProductId);
                product.Quantity = product.Quantity + item.Quantity;
                var updatedProduct = await _productService.Edit(product);
            }
            purchaseOrder.NetTotal = netTotal;
            purchaseOrder = await _purchaseOrderService.Edit(purchaseOrder);
            return CreatedAtRoute("DefaultApi", new { id = purchaseOrder.PurchaseOrderId }, Mapping.ToPurchaseOrderDto(purchaseOrder));

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

        private decimal CalculateAmount(ProductsPurchaseOrder productsSalesOrder)
        {
            var price = productsSalesOrder.OrderPrice;
            var cgst = productsSalesOrder.CGST;
            var sgst = productsSalesOrder.SGST;
            var amt = price + (price * (cgst / 100)) + (price * (sgst / 100));
            return Convert.ToDecimal(amt * productsSalesOrder.Quantity);
        }
    }
}
