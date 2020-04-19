using Client;
using Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ViewModels;
using Web.Common;
using Web.Filters;

namespace Web.Controllers
{
    [SessionTimeout]
    public class PurchaseOrdersController : Controller
    {
        private readonly ICompanyClient _companyClient;
        private readonly IVendorClient _vendorClient;
        private readonly IResourceClient _resourceClient;
        private readonly IProductClient _productClient;
        private readonly IPurchaseOrderClient _purchaseOrderClient;
        private readonly IProductTypeClient _productTypeClient;
        private readonly IManufacturerClient _manufacturerClient;
        private readonly ICategoryClient _categoryClient;
        private readonly IConfig _config;

        public PurchaseOrdersController(ICompanyClient companyClient,
                                     IResourceClient resourceClient,
                                     IProductClient productClient,
                                     IPurchaseOrderClient purchaseOrderClient,
                                     IVendorClient vendorClient,
                                     IProductTypeClient productTypeClient,
                                     IManufacturerClient manufacturerClient,
                                     ICategoryClient categoryClient,
                                     IConfig config
                                     )
        {
            _companyClient = companyClient;
            _resourceClient = resourceClient;
            _productClient = productClient;
            _purchaseOrderClient = purchaseOrderClient;
            _vendorClient = vendorClient;
            _productTypeClient = productTypeClient;
            _manufacturerClient = manufacturerClient;
            _categoryClient = categoryClient;
            _config = config;

        }
        // GET: PurchaseOrders
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _purchaseOrderClient.GetList();

            List<PurchaseOrderInfo> purchaseOrderInfos = null;

            if (result.IsSuccessStatusCode)
            {
                purchaseOrderInfos = await result.Content.ReadAsAsync<List<PurchaseOrderInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(purchaseOrderInfos);
        }

        // GET: PurchaseOrders/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);
        }

        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _purchaseOrderClient.Get(id);

            PurchaseOrderDto purchaseOrderDto = null;

            if (result.IsSuccessStatusCode)
            {
                purchaseOrderDto = await result.Content.ReadAsAsync<PurchaseOrderDto>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            PurchaseOrderInfo purchaseOrderInfo = await GetPurchaseOrderInfo(purchaseOrderDto);
            return View(purchaseOrderInfo);
        }

        private async Task<PurchaseOrderInfo> GetPurchaseOrderInfo(PurchaseOrderDto salesOrderDto)
        {
            PurchaseOrderInfo salesOrderInfo = new PurchaseOrderInfo();
            salesOrderInfo.PurchaseOrderId = salesOrderDto.PurchaseOrderId;
            salesOrderInfo.CompanyId = salesOrderDto.CompanyId;
            salesOrderInfo.ResourceId = salesOrderDto.ResourceId;
            salesOrderInfo.ResourceName = salesOrderDto.ResourceName;
            salesOrderInfo.NetTotal = salesOrderDto.NetTotal;
            salesOrderInfo.InvoiceDate = salesOrderDto.PurchaseDate;
            salesOrderInfo.IsPaid = salesOrderDto.IsPaid ? "Paid" : "Unpaid";
            HttpResponseMessage result = await _companyClient.Get(salesOrderDto.CompanyId);
            Company company = null;
            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }

            salesOrderInfo.CompanyName = company.CompanyName;

            result = await _vendorClient.Get(salesOrderDto.VendorId);
            Vendor customer = null;
            if (result.IsSuccessStatusCode)
            {
                customer = await result.Content.ReadAsAsync<Vendor>();
            }

            salesOrderInfo.VendorName = customer.VendorName;

            salesOrderInfo.productsPurchaseOrder = await getProductsPurchaseOrder(salesOrderDto.ProductsPurchaseOrders.ToList());

            return salesOrderInfo;
        }


        private async Task<List<ProductsPurchaseOrder>> getProductsPurchaseOrder(List<ProductsPurchaseOrderDto> productsSalesOrderDtos)
        {
            List<ProductsPurchaseOrder> productsSalesOrders = new List<ProductsPurchaseOrder>();
            foreach (var productsSalesOrderDto in productsSalesOrderDtos)
            {
                ProductsPurchaseOrder productsSalesOrder = new ProductsPurchaseOrder();
                // productsSalesOrder.CategoryName = productsSalesOrderDto.Products.CategoryName;                
                productsSalesOrder.CGST = productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.CGST / 100);
                productsSalesOrder.CgstPercent = string.Format("({0} %)", Convert.ToInt32(productsSalesOrderDto.CGST));
                // productsSalesOrder.ManufacturerName = productsSalesOrderDto.Products.ManufacturerName;                
                productsSalesOrder.OrderPrice = productsSalesOrderDto.OrderPrice;
                // productsSalesOrder.ProductTypeName = productsSalesOrderDto.Products.ProductTypeName;
                productsSalesOrder.Quantity = productsSalesOrderDto.Quantity;
                productsSalesOrder.SGST = productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.SGST / 100);
                productsSalesOrder.SgstPercent = string.Format("({0} %)", Convert.ToInt32(productsSalesOrderDto.SGST));
                productsSalesOrder.ProductName = productsSalesOrderDto.Products.ProductName;
                productsSalesOrder.WeightInKg = productsSalesOrderDto.Products.WeightInKg;
                productsSalesOrder.RowTotal = (productsSalesOrderDto.OrderPrice + (productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.CGST / 100)) + (productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.SGST / 100))) * productsSalesOrderDto.Quantity;
                productsSalesOrder.ProductDescription = productsSalesOrderDto.Products.ProductDescription;
                HttpResponseMessage result = await _categoryClient.Get(productsSalesOrderDto.Products.CategoryId);
                Category category = null;
                if (result.IsSuccessStatusCode)
                {
                    category = await result.Content.ReadAsAsync<Category>();
                }
                productsSalesOrder.CategoryName = category.CategoryName;

                result = await _manufacturerClient.Get(productsSalesOrderDto.Products.ManufacturerId);
                Manufacturer manufacturer = null;
                if (result.IsSuccessStatusCode)
                {
                    manufacturer = await result.Content.ReadAsAsync<Manufacturer>();
                }
                productsSalesOrder.ManufacturerName = manufacturer.ManufacturerName;

                result = await _productTypeClient.Get(productsSalesOrderDto.Products.ProductTypeId);
                ProductType productType = null;
                if (result.IsSuccessStatusCode)
                {
                    productType = await result.Content.ReadAsAsync<ProductType>();
                }
                productsSalesOrder.ProductTypeName = productType.ProductTypeName;

                productsSalesOrders.Add(productsSalesOrder);
            }


            return productsSalesOrders;
        }

        // GET: PurchaseOrders/Create
        public async Task<ActionResult> Create()
        {
            await BindDropDown();
            return View();
        }

        private async Task BindDropDown()
        {
            List<ProductInfo> products = new List<ProductInfo>();
            HttpResponseMessage result = await _productClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                products = await result.Content.ReadAsAsync<List<ProductInfo>>();
            }
            ViewData["Products"] = products.Select(x => new SelectListItem { Value = x.ProductId.ToString(), Text = x.ProductName + " - " + x.Quantity });


            List<Company> companies = new List<Company>();
            result = await _companyClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                companies = await result.Content.ReadAsAsync<List<Company>>();
            }
            ViewData["Companies"] = companies.Select(x => new SelectListItem { Value = x.CompanyId.ToString(), Text = x.CompanyName });



            List<ResourceInfo> resourceInfos = new List<ResourceInfo>();
            result = await _resourceClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                resourceInfos = await result.Content.ReadAsAsync<List<ResourceInfo>>();
            }
            ViewData["Resources"] = resourceInfos.Select(x => new SelectListItem { Value = x.ResourceId.ToString(), Text = x.ResourceName });

        }

        // POST: PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PurchaseOrder collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (collection.VendorId == null)
                    {
                        var vendorDto = new VendorDto();
                        vendorDto.VendorName = collection.VendorName;
                        vendorDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                        vendorDto.CompanyId = Convert.ToInt32(collection.CompanyId);
                        var result_1 = await _vendorClient.Add(vendorDto);
                        if (result_1 == null)
                        {
                            return RedirectToAction("Timeout", "Login");
                        }
                        var contents_1 = await result_1.Content.ReadAsStringAsync();
                        if (result_1.IsSuccessStatusCode)
                        {
                            VendorInfo vendor = JsonConvert.DeserializeObject<VendorInfo>(contents_1);
                            collection.VendorId = vendor.VendorId;
                        }
                        else
                        {
                            ViewBag.Message = _config.errorMessage();
                            ViewBag.InnerMessage = contents_1;
                            await BindDropDown();
                            return View();
                        }
                    }

                    PurchaseOrderDto purchaseOrderDto = new PurchaseOrderDto();

                    purchaseOrderDto.CompanyId = collection.CompanyId;
                    purchaseOrderDto.IsPaid = collection.IsPaid;
                    purchaseOrderDto.ResourceId = collection.ResourceId;
                    purchaseOrderDto.VendorId = Convert.ToInt32(collection.VendorId);
                    List<ProductsPurchaseOrderDto> productsSalesOrderDtos = new List<ProductsPurchaseOrderDto>();
                    foreach (var item in collection.productsPurchaseOrders)
                    {
                        ProductsPurchaseOrderDto productsSalesOrderDto = new ProductsPurchaseOrderDto();
                        productsSalesOrderDto.ProductId = item.ProductId;
                        productsSalesOrderDto.Quantity = Convert.ToInt32(item.Quantity);
                        productsSalesOrderDto.OrderPrice = item.OrderPrice;

                        productsSalesOrderDtos.Add(productsSalesOrderDto);
                    }

                    purchaseOrderDto.ProductsPurchaseOrders = productsSalesOrderDtos;
                    HttpResponseMessage result = await _purchaseOrderClient.Add(purchaseOrderDto);

                    return await checkResult(result, _config.getCreatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault()[0].ErrorMessage;
                    await BindDropDown();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }
        }

        private async Task<ActionResult> checkResult(HttpResponseMessage result, string innerMessage)
        {
            if (result == null)
            {
                return RedirectToAction("Timeout", "Login");
            }

            var contents = await result.Content.ReadAsStringAsync();

            if (result.IsSuccessStatusCode)
            {
                PurchaseOrder purchaseOrder = JsonConvert.DeserializeObject<PurchaseOrder>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = purchaseOrder.PurchaseOrderId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }


        // GET: PurchaseOrders/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PurchaseOrders/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: PurchaseOrders/Paid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Paid(int id)
        {
            try
            {
                HttpResponseMessage result = await _purchaseOrderClient.Paid(id, null);
                return await checkResult(result, _config.getUpdatedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                return View();
            }
        }

        // GET: PurchaseOrders/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PurchaseOrders/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
