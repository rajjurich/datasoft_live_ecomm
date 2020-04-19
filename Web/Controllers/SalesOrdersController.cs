using Dtos;
using ViewModels;
using Client;
using Web.Common;
using Web.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Logger;

namespace Web.Controllers
{
    [SessionTimeout]
    public class SalesOrdersController : BaseController
    {
        private readonly ICompanyClient _companyClient;
        private readonly IResourceClient _resourceClient;
        private readonly ICustomerClient _customerClient;
        private readonly ISalesOrderClient _salesOrderClient;
        private readonly IProductTypeClient _productTypeClient;
        private readonly IManufacturerClient _manufacturerClient;
        private readonly ICategoryClient _categoryClient;
        private readonly IProductClient _productClient;
        private readonly IConfig _config;        
        public SalesOrdersController(ICompanyClient companyClient,
                                     IResourceClient resourceClient,
                                     ICustomerClient customerClient,
                                     ISalesOrderClient salesOrderClient,
                                     IProductTypeClient productTypeClient,
                                     IManufacturerClient manufacturerClient,
                                     ICategoryClient categoryClient,
                                     IProductClient productClient,
                                     IConfig config)
        {
            _companyClient = companyClient;
            _resourceClient = resourceClient;
            _customerClient = customerClient;
            _salesOrderClient = salesOrderClient;
            _productTypeClient = productTypeClient;
            _manufacturerClient = manufacturerClient;
            _categoryClient = categoryClient;
            _productClient = productClient;
            _config = config;            
        }
        // GET: SalesOrders
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _salesOrderClient.GetList();
            
            List<SalesOrderInfo> salesOrderInfos = null;

            if (result.IsSuccessStatusCode)
            {                
                salesOrderInfos = await result.Content.ReadAsAsync<List<SalesOrderInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(salesOrderInfos);
        }

        // GET: SalesOrders/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);
        }

        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _salesOrderClient.Get(id);

            SalesOrderDto salesOrderDto = null;

            if (result.IsSuccessStatusCode)
            {
                salesOrderDto = await result.Content.ReadAsAsync<SalesOrderDto>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            SalesOrderInfo salesOrderInfo = await GetSalesOrderInfo(salesOrderDto);
            return View(salesOrderInfo);
        }

        private async Task<SalesOrderInfo> GetSalesOrderInfo(SalesOrderDto salesOrderDto)
        {
            SalesOrderInfo salesOrderInfo = new SalesOrderInfo();
            salesOrderInfo.SalesOrderId = salesOrderDto.SalesOrderId;
            salesOrderInfo.CompanyId = salesOrderDto.CompanyId;
            salesOrderInfo.ResourceId = salesOrderDto.ResourceId;
            salesOrderInfo.ResourceName = salesOrderDto.ResourceName;
            salesOrderInfo.NetTotal = salesOrderDto.NetTotal;
            salesOrderInfo.InvoiceDate = salesOrderDto.InvoiceDate;
            salesOrderInfo.IsPaid = salesOrderDto.IsPaid ? "Paid" : "Unpaid";
            HttpResponseMessage result = await _companyClient.Get(salesOrderDto.CompanyId);
            Company company = null;
            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }

            salesOrderInfo.CompanyName = company.CompanyName;

            result = await _customerClient.Get(salesOrderDto.CustomerId);
            Customer customer = null;
            if (result.IsSuccessStatusCode)
            {
                customer = await result.Content.ReadAsAsync<Customer>();
            }

            salesOrderInfo.CustomerName = customer.CustomerName;
            salesOrderInfo.FullAddress = customer.Addresses[0].FullAddress;

            salesOrderInfo.productsSalesOrders = await getProductsSalesOrder(salesOrderDto.ProductsSalesOrders.ToList());

            return salesOrderInfo;
        }

        private async Task<List<ProductsSalesOrder>> getProductsSalesOrder(List<ProductsSalesOrderDto> productsSalesOrderDtos)
        {
            List<ProductsSalesOrder> productsSalesOrders = new List<ProductsSalesOrder>();
            foreach (var productsSalesOrderDto in productsSalesOrderDtos)
            {
                ProductsSalesOrder productsSalesOrder = new ProductsSalesOrder();
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
                productsSalesOrder.RowTotal = productsSalesOrderDto.OrderPrice + (productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.CGST / 100)) + (productsSalesOrderDto.OrderPrice * (productsSalesOrderDto.SGST / 100));
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

        // GET: SalesOrders/Create
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


        // POST: SalesOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SalesOrder collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (collection.CustomerId == null)
                    {
                        var customerDto = new CustomerDto();
                        customerDto.CustomerName = collection.CustomerName;
                        customerDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                        customerDto.CompanyId = Convert.ToInt32(collection.CompanyId);
                        var result_1 = await _customerClient.Add(customerDto);
                        if (result_1 == null)
                        {
                            return RedirectToAction("Timeout", "Login");
                        }
                        var contents_1 = await result_1.Content.ReadAsStringAsync();
                        if (result_1.IsSuccessStatusCode)
                        {
                            CustomerInfo customer = JsonConvert.DeserializeObject<CustomerInfo>(contents_1);
                            collection.CustomerId = customer.CustomerId;
                        }
                        else
                        {
                            ViewBag.Message = _config.errorMessage();
                            ViewBag.InnerMessage = contents_1;
                            await BindDropDown();
                            return View();
                        }
                    }
                    List<ProductsSalesOrderDto> productsSalesOrderDtos = new List<ProductsSalesOrderDto>();
                    foreach (var item in collection.productsSalesOrders)
                    {
                        ProductsSalesOrderDto productsSalesOrderDto = new ProductsSalesOrderDto();
                        productsSalesOrderDto.ProductId = item.ProductId;
                        productsSalesOrderDto.Quantity = Convert.ToInt32(item.Quantity);
                        productsSalesOrderDto.OrderPrice = item.OrderPrice;
                        productsSalesOrderDto.CGST = item.CGST;
                        productsSalesOrderDto.SGST = item.SGST;

                        productsSalesOrderDtos.Add(productsSalesOrderDto);
                    }

                    var salesOrderDto = new SalesOrderDto();
                    salesOrderDto.CompanyId = collection.CompanyId;
                    salesOrderDto.CustomerId = Convert.ToInt32(collection.CustomerId);
                    salesOrderDto.ResourceId = collection.ResourceId;
                    salesOrderDto.IsPaid = collection.IsPaid;
                    salesOrderDto.ProductsSalesOrders = productsSalesOrderDtos;

                    HttpResponseMessage result = await _salesOrderClient.Add(salesOrderDto);

                    return await checkResult(result, _config.getCreatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
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
                SalesOrderInfo salesOrderInfo = JsonConvert.DeserializeObject<SalesOrderInfo>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = salesOrderInfo.SalesOrderId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }
        // GET: SalesOrders/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SalesOrders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, SalesOrderDto collection)
        {
            try
            {
                HttpResponseMessage result = await _salesOrderClient.Edit(id, collection);
                return await checkResult(result, _config.getUpdatedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }
        }

        // POST: SalesOrders/Paid/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Paid(int id)
        {
            try
            {
                HttpResponseMessage result = await _salesOrderClient.Paid(id, null);
                return await checkResult(result, _config.getUpdatedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                return View();
            }
        }

        // GET: SalesOrders/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SalesOrders/Delete/5
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
