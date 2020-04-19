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
    public class VendorsController : BaseController
    {
        private readonly IVendorClient _vendorClient;
        private readonly ICompanyClient _companyClient;
        private readonly IConfig _config;
        public VendorsController(IVendorClient vendorClient,
                                 ICompanyClient companyClient,
                                 IConfig config)
        {
            _vendorClient = vendorClient;
            _companyClient = companyClient;
            _config = config;
        }
        // GET: Vendors
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _vendorClient.GetList();
            List<VendorInfo> vendorInfos = null;

            if (result.IsSuccessStatusCode)
            {
                vendorInfos = await result.Content.ReadAsAsync<List<VendorInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(vendorInfos);            
        }

        // GET: Vendors/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);
        }

        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _vendorClient.Get(id);

            Vendor vendor = null;

            if (result.IsSuccessStatusCode)
            {
                vendor = await result.Content.ReadAsAsync<Vendor>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            VendorInfo vendorInfo = await GetVendorInfo(vendor);
            return View(vendorInfo);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _vendorClient.Get(id);

            Vendor vendor = null;

            if (result.IsSuccessStatusCode)
            {
                vendor = await result.Content.ReadAsAsync<Vendor>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(vendor);
        }

        private async Task<VendorInfo> GetVendorInfo(Vendor vendor)
        {
            VendorInfo vendorInfo = new VendorInfo();
            vendorInfo.VendorId = vendor.VendorId;
            vendorInfo.VendorName = vendor.VendorName;
            vendorInfo.EmailId = vendor.EmailId;
            vendorInfo.GSTNumber = vendor.GSTNumber;
            vendorInfo.PAN = vendor.PAN;
            vendorInfo.PrimaryMobileNumber = vendor.PrimaryMobileNumber;
            vendorInfo.SecondaryMobileNumber = vendor.SecondaryMobileNumber;

            HttpResponseMessage result = await _companyClient.Get(vendor.CompanyId);
            Company company = null;
            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }
           
            vendorInfo.CompanyName = company.CompanyName;
            return vendorInfo;
        }

        // GET: Vendors/Create
        public async Task<ActionResult> Create()
        {
            await BindDropDown();
            return View();
        }

        private async Task BindDropDown()
        {
            List<Company> companies = new List<Company>();
            HttpResponseMessage result = await _companyClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                companies = await result.Content.ReadAsAsync<List<Company>>();
            }
            ViewData["Companies"] = companies.Select(x => new SelectListItem { Value = x.CompanyId.ToString(), Text = x.CompanyName });
        }

        // POST: Vendors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Vendor collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var vendorDto = new VendorDto();
                    vendorDto.VendorName = collection.VendorName;
                    vendorDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        vendorDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        vendorDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    vendorDto.EmailId = collection.EmailId;
                    vendorDto.PAN = collection.PAN;
                    vendorDto.GSTNumber = collection.GSTNumber;
                    vendorDto.CompanyId = collection.CompanyId;

                    HttpResponseMessage result = await _vendorClient.Add(vendorDto);

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
                VendorInfo vendorInfo = JsonConvert.DeserializeObject<VendorInfo>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = vendorInfo.VendorId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }

        // GET: Vendors/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await BindDropDown();
            return await getSingle(id);
        }

        // POST: Vendors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Vendor collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var vendorDto = new VendorDto();
                    vendorDto.VendorId = collection.VendorId;
                    vendorDto.VendorName = collection.VendorName;
                    vendorDto.PrimaryMobileNumber = Convert.ToInt64(collection.PrimaryMobileNumber);
                    if (string.IsNullOrWhiteSpace(collection.SecondaryMobileNumber))
                    {
                        vendorDto.SecondaryMobileNumber = null;
                    }
                    else
                    {
                        vendorDto.SecondaryMobileNumber = Convert.ToInt64(collection.SecondaryMobileNumber);
                    }
                    vendorDto.PAN = collection.PAN;
                    vendorDto.GSTNumber = collection.GSTNumber;
                    vendorDto.EmailId = collection.EmailId;
                    vendorDto.CompanyId = collection.CompanyId;

                    HttpResponseMessage result = await _vendorClient.Edit(id, vendorDto);
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

        // GET: Vendors/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _vendorClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Vendors", new { id = id });
        }

        // POST: Vendors/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Vendor collection)
        {
            try
            {
                HttpResponseMessage result = await _vendorClient.Delete(id);
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