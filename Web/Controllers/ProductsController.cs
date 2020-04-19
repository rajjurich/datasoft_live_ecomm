using Dtos;
using ViewModels;
using Client;
using Web.Common;
using Web.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;

namespace Web.Controllers
{
    [SessionTimeout]
    public class ProductsController : BaseController
    {
        private readonly IProductClient _productClient;
        private readonly IProductTypeClient _productTypeClient;
        private readonly IManufacturerClient _manufacturerClient;
        private readonly ICategoryClient _categoryClient;
        private readonly ICompanyClient _companyClient;
        private readonly IConfig _config;
        public ProductsController(IProductClient productClient,
                                        IProductTypeClient productTypeClient,
                                        IManufacturerClient manufacturerClient,
                                        ICategoryClient categoryClient,
                                        ICompanyClient companyClient,
                                        IConfig config
                                        )
        {
            _productClient = productClient;
            _productTypeClient = productTypeClient;
            _manufacturerClient = manufacturerClient;
            _categoryClient = categoryClient;
            _companyClient = companyClient;
            _config = config;
        }
        // GET: Products
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _productClient.GetList();
            List<ProductInfo> products = null;

            if (result.IsSuccessStatusCode)
            {
                products = await result.Content.ReadAsAsync<List<ProductInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(products);            
        }        

        // GET: Products/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);            
        }

        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _productClient.Get(id);

            Product product = null;

            if (result.IsSuccessStatusCode)
            {
                product = await result.Content.ReadAsAsync<Product>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            ProductInfo productInfo = await GetProductInfo(product);
            return View(productInfo);
        }        

        private async Task<ProductInfo> GetProductInfo(Product product)
        {
            ProductInfo productInfo = new ProductInfo();
            productInfo.ProductId = product.ProductId;
            productInfo.ProductName = product.ProductName;
            productInfo.WeightInKg = product.WeightInKg;
            productInfo.ProductDescription = product.ProductDescription;
            productInfo.Quantity = product.Quantity;

            HttpResponseMessage result = await _categoryClient.Get(product.CategoryId);
            Category category = null;
            if (result.IsSuccessStatusCode)
            {
                category = await result.Content.ReadAsAsync<Category>();
            }            
            productInfo.CategoryName = category.CategoryName;

            result = await _manufacturerClient.Get(product.ManufacturerId);
            Manufacturer manufacturer = null;
            if (result.IsSuccessStatusCode)
            {
                manufacturer = await result.Content.ReadAsAsync<Manufacturer>();
            }           
            productInfo.ManufacturerName = manufacturer.ManufacturerName;
           
            result = await _productTypeClient.Get(product.ProductTypeId);
            ProductType productType = null;
            if (result.IsSuccessStatusCode)
            {
                productType = await result.Content.ReadAsAsync<ProductType>();
            }            
            productInfo.ProductTypeName = productType.ProductTypeName;

            result = await _companyClient.Get(product.CompanyId);
            Company company = null;
            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }
            
            productInfo.CompanyName = company.CompanyName;

            return productInfo;
        }

        // GET: Products/Create
        public async Task<ActionResult> Create()
        {
            await BindDropDown();

            return View();
        }

        private async Task BindDropDown()
        {            
            List<ProductType> productTypes = new List<ProductType>();
            HttpResponseMessage result = await _productTypeClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                productTypes = await result.Content.ReadAsAsync<List<ProductType>>();
            }            
            ViewData["ProductTypes"] = productTypes.Select(x => new SelectListItem { Value = x.ProductTypeId.ToString(), Text = x.ProductTypeName });

            List<Manufacturer> manufacturers = new List<Manufacturer>();
            result = await _manufacturerClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                manufacturers = await result.Content.ReadAsAsync<List<Manufacturer>>();
            }
            ViewData["Manufacturers"] = manufacturers.Select(x => new SelectListItem { Value = x.ManufacturerId.ToString(), Text = x.ManufacturerName });

            List<Category> categories = new List<Category>();
            result = await _categoryClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                categories = await result.Content.ReadAsAsync<List<Category>>();
            }            
            ViewData["Categories"] = categories.Select(x => new SelectListItem { Value = x.CategoryId.ToString(), Text = x.CategoryName });

            List<Company> companies = new List<Company>();
            result = await _companyClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                companies = await result.Content.ReadAsAsync<List<Company>>();
            }            
            ViewData["Companies"] = companies.Select(x => new SelectListItem { Value = x.CompanyId.ToString(), Text = x.CompanyName });
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var productDto = new ProductDto();
                    productDto.ProductName = collection.ProductName;
                    productDto.WeightInKg = collection.WeightInKg;                   
                    productDto.ProductDescription = collection.ProductDescription;
                    productDto.ProductId = collection.ProductId;
                    productDto.ProductTypeId = collection.ProductTypeId;
                    productDto.ManufacturerId = collection.ManufacturerId;
                    productDto.CategoryId = collection.CategoryId;
                    productDto.CompanyId = collection.CompanyId;

                    HttpResponseMessage result = await _productClient.Add(productDto);

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
                Product product = JsonConvert.DeserializeObject<Product>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = product.ProductId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }
        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await BindDropDown();
            return await getSingle(id);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _productClient.Get(id);

            Product product = null;

            if (result.IsSuccessStatusCode)
            {
                product = await result.Content.ReadAsAsync<Product>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(product);
        }
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Product collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var productDto = new ProductDto();
                    productDto.ProductId = collection.ProductId;
                    productDto.ProductName = collection.ProductName;
                    productDto.WeightInKg = collection.WeightInKg;                    
                    productDto.ProductDescription = collection.ProductDescription;
                    productDto.CategoryId = collection.CategoryId;
                    productDto.ManufacturerId = collection.ManufacturerId;
                    productDto.ProductId = collection.ProductId;
                    productDto.CompanyId = collection.CompanyId;

                    HttpResponseMessage result = await _productClient.Edit(id, productDto);
                    return await checkResult(result, _config.getUpdatedInnerMessage());
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

        // GET: Products/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _productClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Products", new { id = id });
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product collection)
        {
            try
            {
                HttpResponseMessage result = await _productClient.Delete(id);
                return await checkResult(result, _config.getDeletedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                await BindDropDown();
                return View();
            }            
        }
    }
}
