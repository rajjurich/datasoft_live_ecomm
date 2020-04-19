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
    public class ManufacturersController : BaseController
    {
        private readonly IManufacturerClient _manufacturerClient;
        private readonly IConfig _config;
        public ManufacturersController(IManufacturerClient manufacturerClient,
                                       IConfig config)
        {
            _manufacturerClient = manufacturerClient;
            _config = config;
        }
        // GET: Manufacturers
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _manufacturerClient.GetList();
            List<Manufacturer> manufacturers = null;

            if (result.IsSuccessStatusCode)
            {
                manufacturers = await result.Content.ReadAsAsync<List<Manufacturer>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(manufacturers);           
        }

        // GET: Manufacturers/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingle(id);          
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _manufacturerClient.Get(id);

            Manufacturer manufacturer = null;

            if (result.IsSuccessStatusCode)
            {
                manufacturer = await result.Content.ReadAsAsync<Manufacturer>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(manufacturer);
        }

        // GET: Manufacturers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Manufacturers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Manufacturer collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var manufacturerDto = new ManufacturerDto();
                    manufacturerDto.ManufacturerName = collection.ManufacturerName;

                    HttpResponseMessage result = await _manufacturerClient.Add(manufacturerDto);

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
                Manufacturer manufacturer = JsonConvert.DeserializeObject<Manufacturer>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = manufacturer.ManufacturerId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                return View();
            }
        }
        // GET: Manufacturers/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getSingle(id);
        }

        // POST: Manufacturers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Manufacturer collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var manufacturerDto = new ManufacturerDto();
                    manufacturerDto.ManufacturerId = collection.ManufacturerId;
                    manufacturerDto.ManufacturerName = collection.ManufacturerName;

                    HttpResponseMessage result = await _manufacturerClient.Edit(id, manufacturerDto);
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

        // GET: Manufacturers/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _manufacturerClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Manufacturers", new { id = id });
        }

        // POST: Manufacturers/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Manufacturer collection)
        {
            try
            {
                HttpResponseMessage result = await _manufacturerClient.Delete(id);
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