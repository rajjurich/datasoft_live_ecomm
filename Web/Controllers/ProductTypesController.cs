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

namespace Web.Controllers
{
    [SessionTimeout]
    public class ProductTypesController : BaseController
    {
        private readonly IProductTypeClient _productTypeClient;
        private readonly IConfig _config;
        public ProductTypesController(IProductTypeClient productTypeClient,
                                  IConfig config)
        {
            _productTypeClient = productTypeClient;
            _config = config;
        }
        // GET: ProductTypes
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _productTypeClient.GetList();
            List<ProductType> productType = null;

            if (result.IsSuccessStatusCode)
            {
                productType = await result.Content.ReadAsAsync<List<ProductType>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(productType);            
        }

        // GET: ProductTypes/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingle(id);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _productTypeClient.Get(id);

            ProductType productType = null;

            if (result.IsSuccessStatusCode)
            {
                productType = await result.Content.ReadAsAsync<ProductType>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(productType);
        }

        // GET: ProductTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductType collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var productTypeDto = new ProductTypeDto();
                    productTypeDto.ProductTypeName = collection.ProductTypeName;

                    HttpResponseMessage result = await _productTypeClient.Add(productTypeDto);

                    return await checkResult(result, _config.getCreatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
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
                ProductType productType = JsonConvert.DeserializeObject<ProductType>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = productType.ProductTypeId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                return View();
            }
        }
        // GET: Products/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getSingle(id);
        }

        // POST: ProductTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ProductType collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var productTypeDto = new ProductTypeDto();
                    productTypeDto.ProductTypeId = collection.ProductTypeId;
                    productTypeDto.ProductTypeName = collection.ProductTypeName;

                    HttpResponseMessage result = await _productTypeClient.Edit(id, productTypeDto);
                    return await checkResult(result, _config.getUpdatedInnerMessage());
                }
                else
                {
                    ViewBag.Message = _config.errorMessage();
                    ViewBag.InnerMessage = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                return View();
            }            
        }

        // GET: ProductTypes/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _productTypeClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "ProductTypes", new { id = id });
        }

        // POST: Products/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Product collection)
        {
            try
            {
                HttpResponseMessage result = await _productTypeClient.Delete(id);
                return await checkResult(result, _config.getDeletedInnerMessage());
            }
            catch (Exception ex)
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = ex.Message;
                return View();
            }            
        }
    }
}