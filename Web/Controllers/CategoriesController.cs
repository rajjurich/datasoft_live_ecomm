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
    public class CategoriesController : BaseController
    {
        private ICategoryClient _categoryClient;
        private readonly IConfig _config;
        public CategoriesController(ICategoryClient categoryClient,
                                    IConfig config)
        {
            _categoryClient = categoryClient;
            _config = config;
        }
        // GET: Categories
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _categoryClient.GetList();
            List<Category> categories = null;

            if (result.IsSuccessStatusCode)
            {
                categories = await result.Content.ReadAsAsync<List<Category>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingle(id);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _categoryClient.Get(id);

            Category category = null;

            if (result.IsSuccessStatusCode)
            {
                category = await result.Content.ReadAsAsync<Category>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(category);
        }

        // GET: Categories/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var categoryDto = new CategoryDto();
                    categoryDto.CategoryName = collection.CategoryName;

                    HttpResponseMessage result = await _categoryClient.Add(categoryDto);

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
                Category category = JsonConvert.DeserializeObject<Category>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = category.CategoryId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                return View();
            }
        }

        // GET: Categories/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getSingle(id);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Category collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var categoryDto = new CategoryDto();
                    categoryDto.CategoryId = collection.CategoryId;
                    categoryDto.CategoryName = collection.CategoryName;

                    HttpResponseMessage result = await _categoryClient.Edit(id, categoryDto);
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

        // GET: Categories/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _categoryClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Categories", new { id = id });
        }

        // POST: Categories/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Category collection)
        {
            try
            {
                HttpResponseMessage result = await _categoryClient.Delete(id);
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
