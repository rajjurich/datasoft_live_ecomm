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
    public class MenuController : BaseController
    {
        private IMenuClient _menuClient;
        private readonly IConfig _config;

        public MenuController(IMenuClient menuClient,
                                    IConfig config)
        {
            _menuClient = menuClient;
            _config = config;
        }
        // GET: Menu
        public async Task<ActionResult> Index()
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];

            HttpResponseMessage result = await _menuClient.GetAllList();
            List<Menu> menus = null;

            if (result.IsSuccessStatusCode)
            {
                menus = await result.Content.ReadAsAsync<List<Menu>>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(menus);
        }

        // GET: Menu/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.Message = TempData["Message"];
            ViewBag.InnerMessage = TempData["InnerMessage"];
            return await getSingle(id);
        }

        private async Task<ActionResult> getSingle(int id)
        {
            HttpResponseMessage result = await _menuClient.Get(id);

            Menu menu = null;

            if (result.IsSuccessStatusCode)
            {
                menu = await result.Content.ReadAsAsync<Menu>();
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                return RedirectToAction("Error", "Login");
            }
            else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToAction("Timeout", "Login");
            }

            return View(menu);
        }

        // GET: Menu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Menu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Menu collection)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    HttpResponseMessage result = await _menuClient.Add(collection);

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
                Menu menu = JsonConvert.DeserializeObject<Menu>(contents);
                TempData["Message"] = _config.successMessage();
                TempData["InnerMessage"] = innerMessage;
                return RedirectToAction("Details", new { id = menu.MenuId });
            }
            else
            {
                ViewBag.Message = _config.errorMessage();
                ViewBag.InnerMessage = contents;
                return View();
            }
        }

        // GET: Menu/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            return await getSingle(id);
        }

        // POST: Menu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Menu collection)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    HttpResponseMessage result = await _menuClient.Edit(id, collection);
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

        // GET: Menu/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage result = await _menuClient.Get(id);
            if (!(result.IsSuccessStatusCode))
            {
                return RedirectToAction("Timeout", "Login");
            }
            return RedirectToAction("Details", "Menus", new { id = id });
        }

        // POST: Menu/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Menu collection)
        {
            try
            {
                HttpResponseMessage result = await _menuClient.Delete(id);
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
