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
    public class ResourcesController : BaseController
    {
        private readonly IResourceClient _resourceClient;
        private readonly IRegisterClient _registerClient;
        private readonly ICompanyClient _companyClient;
        private readonly IRoleClient _roleClient;
        private readonly IConfig _config;
        public ResourcesController(IResourceClient resourceClient,
                                   ICompanyClient companyClient,
                                   IRoleClient roleClient,
                                   IConfig config,
                                   IRegisterClient registerClient)
        {
            _resourceClient = resourceClient;
            _companyClient = companyClient;
            _roleClient = roleClient;
            _config = config;
            _registerClient = registerClient;
        }
        // GET: Resources
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _resourceClient.GetList();
            List<ResourceInfo> resources = null;

            if (result.IsSuccessStatusCode)
            {
                resources = await result.Content.ReadAsAsync<List<ResourceInfo>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(resources);            
        }

        // GET: Resources/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingleInfo(id);
        }

        private async Task<ActionResult> getSingleInfo(int id)
        {
            HttpResponseMessage result = await _resourceClient.Get(id);

            Resource resource = null;

            if (result.IsSuccessStatusCode)
            {
                resource = await result.Content.ReadAsAsync<Resource>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            ResourceInfo resourceInfo = await GetResourceInfo(resource);
            return View(resourceInfo);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _resourceClient.Get(id);

            Resource resource = null;

            if (result.IsSuccessStatusCode)
            {
                resource = await result.Content.ReadAsAsync<Resource>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(resource);
        }
        private async Task<ResourceInfo> GetResourceInfo(Resource resource)
        {
            ResourceInfo resourceInfo = new ResourceInfo();
            resourceInfo.CompanyId = resource.CompanyId;
            resourceInfo.ResourceId = resource.ResourceId;
            resourceInfo.ResourceName = resource.ResourceName;
            resourceInfo.RoleId = resource.RoleId;

            HttpResponseMessage result = await _companyClient.Get(resource.CompanyId);
            Company company = null;
            if (result.IsSuccessStatusCode)
            {
                company = await result.Content.ReadAsAsync<Company>();
            }
            
            resourceInfo.CompanyName = company.CompanyName;


            result = await _roleClient.Get(resource.RoleId);
            Role role = null;
            if (result.IsSuccessStatusCode)
            {
                role = await result.Content.ReadAsAsync<Role>();
            }

            resourceInfo.Role = role.RoleName;

            return resourceInfo;
        }

        // GET: Resources/Create
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

            List<Role> roles = new List<Role>();
            result = await _roleClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                roles = await result.Content.ReadAsAsync<List<Role>>();
            }            
            ViewData["Roles"] = roles.Select(x => new SelectListItem { Value = x.RoleId.ToString(), Text = x.RoleName });
        }

        // POST: Resources/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Resource collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resourceDto = new ResourceDto();
                    resourceDto.ResourceName = collection.ResourceName;
                    resourceDto.CompanyId = collection.CompanyId;
                    resourceDto.RoleId = collection.RoleId;
                    resourceDto.Email = collection.Email;
                    resourceDto.Password = collection.Password;
                    resourceDto.ConfirmPassword = collection.ConfirmPassword;

                    Role role = new Role();
                    HttpResponseMessage result = await _roleClient.Get(collection.RoleId);
                    if (result.IsSuccessStatusCode)
                    {
                        role = await result.Content.ReadAsAsync<Role>();
                    }
                    resourceDto.RoleName = role.RoleName;

                    result = await _registerClient.Register(resourceDto);
                    if (result.IsSuccessStatusCode)
                    {
                        result = await _resourceClient.Add(resourceDto);
                    }

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
                ResourceInfo resourceInfo = JsonConvert.DeserializeObject<ResourceInfo>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = resourceInfo.ResourceId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                await BindDropDown();
                return View();
            }
        }

        // GET: Resources/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            await BindDropDown();
            return await getSingle(id);
        }

        // POST: Resources/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Resource collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resourceDto = new ResourceDto();
                    resourceDto.ResourceId = collection.ResourceId;
                    resourceDto.ResourceName = collection.ResourceName;
                    resourceDto.CompanyId = collection.CompanyId;
                    resourceDto.RoleId = collection.RoleId;

                    HttpResponseMessage result = await _resourceClient.Edit(id, resourceDto);
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

        // GET: Resources/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _resourceClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Resources", new { id = id });
        }

        // POST: Resources/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Resource collection)
        {
            try
            {
                HttpResponseMessage result = await _resourceClient.Delete(id);
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