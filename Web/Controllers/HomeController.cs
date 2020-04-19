using Web.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Client;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using ViewModels;
using System.Configuration;

namespace Web.Controllers
{
    [SessionTimeout]
    public class HomeController : BaseController
    {
        private readonly IMenuClient _menuClient;
        private string _web = ConfigurationManager.AppSettings["web"];
        public HomeController(IMenuClient menuClient)
        {
            _menuClient = menuClient;
        }
        // GET: Home
        //[System.Web.Mvc.OutputCache(Duration = 60)]
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage result = await _menuClient.GetList();
            if (result.IsSuccessStatusCode)
            {
                Session["menu"] = await getMenuString(result);
            }
            else
            {
                return Redirect("~/Login/Index?message=" + await result.Content.ReadAsStringAsync());
            }
            return View();
        }
        private async Task<string> getMenuString(HttpResponseMessage result)
        {
            StringBuilder sb = new StringBuilder();
            // HttpResponseMessage result = await _menuClient.GetList();
            List<Menu> menus = null;

            menus = await result.Content.ReadAsAsync<List<Menu>>();
            foreach (var value in menus)
            {
                if (value.Submenus.Count > 0 && value.ParentMenu == null)
                {
                    //if (value.Title == "Commercials" || value.Title == "Inventory")
                    //{
                    //    sb.Append("<li class='three-column'>");
                    //}
                    //else if (value.Title == "Orders" || value.Title == "Core Config")
                    //{
                    //    sb.Append("<li class='two-column'>");
                    //}
                    //else
                    //{
                    //    sb.Append("<li>");
                    //}
                    sb.Append("<li>");
                    sb.Append("<a class='has-arrow waves-effect waves-dark' href='#' aria-expanded='false'><i class=" + value.Icon + "></i><span class='hide-menu'>" + value.Title + "</span></a>");
                    sb.Append("<ul aria-expanded='false' class='collapse'>");

                    foreach (var subvalue in value.Submenus)
                    {
                        sb.Append("<li>");
                        if (subvalue.Submenus.Count > 0)
                        {
                            sb.Append("<a class='has-arrow' href='#' aria-expanded='false'>" + subvalue.Title + "</a>");
                            sb.Append("<ul aria-expanded='false' class='collapse'>");

                            foreach (var childvalue in subvalue.Submenus)
                            {
                                sb.Append("<li><a href=" + _web + childvalue.URL + ">" + childvalue.Title + "</a></li>");
                            }

                            sb.Append("</ul>");
                        }
                        else
                        {
                            sb.Append("<a href=" + _web + subvalue.URL + ">" + subvalue.Title + "</a>");
                        }
                        sb.Append("</li>");
                    }



                    sb.Append("</ul>");
                }
            }

            //if (result.IsSuccessStatusCode)
            //{

            //}
            //else if (result.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            //{
            //    return result.Content.ToString();
            //}
            //else if (result.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //{
            //    return result.ToString();
            //}

            // return View(menus);
            var x = sb.ToString();
            return sb.ToString();
        }
    }
}