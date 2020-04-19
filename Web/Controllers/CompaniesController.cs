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
    public class CompaniesController : BaseController
    {
        private readonly ICompanyClient _companyClient;
        private readonly IConfig _config;
        public CompaniesController(ICompanyClient companyClient,
                                   IConfig config)
        {
            _companyClient = companyClient;
            _config = config;
        }
        // GET: Companies
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _companyClient.GetList();
            List<Company> companies = null;

            if (result.IsSuccessStatusCode)
            {
                companies = await result.Content.ReadAsAsync<List<Company>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(companies);           
        }

        // GET: Companies/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingle(id);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _companyClient.Get(id);

            Company company = null;

            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Company collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var companyDto = new CompanyDto();
                    companyDto.CompanyName = collection.CompanyName;
                    companyDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        companyDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        companyDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    companyDto.EmailId = collection.EmailId;
                    companyDto.PAN = collection.PAN;
                    companyDto.GSTNumber = collection.GSTNumber;
                    

                    HttpResponseMessage result = await _companyClient.Add(companyDto);

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
                Company company = JsonConvert.DeserializeObject<Company>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = company.CompanyId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                return View();
            }
        }

        // GET: Companies/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getSingle(id);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Company collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var companyDto = new CompanyDto();
                    companyDto.CompanyId = collection.CompanyId;
                    companyDto.CompanyName = collection.CompanyName;
                    companyDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        companyDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        companyDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    companyDto.PAN = collection.PAN;
                    companyDto.GSTNumber = collection.GSTNumber;
                    companyDto.EmailId = collection.EmailId;

                    HttpResponseMessage result = await _companyClient.Edit(id, companyDto);
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

        // GET: Companies/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _companyClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Companies", new { id = id });            
        }

        // POST: Companies/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Company collection)
        {
            try
            {
                HttpResponseMessage result = await _companyClient.Delete(id);
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