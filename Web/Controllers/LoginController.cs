using ViewModels;
using Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Logger;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;

namespace Web.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ILoginClient _loginClient;        
        public LoginController(ILoginClient loginClient)
        {
            _loginClient = loginClient;           
        }
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Message = Request.QueryString["message"];
            return View();
        }
        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Login login)
        {
            var result = await _loginClient.Authenticate(login);
            var contents = await result.Content.ReadAsStringAsync();
            string jsonString = await result.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject(jsonString);
            if (result.IsSuccessStatusCode)
            {               
                Session["accessToken"] = ((dynamic)responseData).access_token;
                Session["username"]=((dynamic)responseData).userName;
                Session.Timeout = ConfigurationManager.AppSettings["TimeOut"] == null ? 10 : Convert.ToInt32(Convert.ToString(ConfigurationManager.AppSettings["TimeOut"]));
                return RedirectToAction("Index", "Home");
            }            
            else
            {
                ViewBag.Message = ((dynamic)responseData).error_description;
                return View();
            }           
        }
        // GET: Logout
        [HttpGet]
        public ActionResult Logout()
        {
            EndSession();
            return Redirect("~/Login/Index?message=Logged out successfully");
        }
        // GET: TokenExpired
        [HttpGet]
        public ActionResult Timeout()
        {
            EndSession();
            return Redirect("~/Login/Index?message=Session expired");
        }
        // GET: NotFound
        [HttpGet]
        public ActionResult NotFound()
        {
            EndSession();
            return Redirect("~/Login/Index?message=Page not found");
        }
        // GET: Error
        [HttpGet]
        public ActionResult Error()
        {
            EndSession();
            return Redirect("~/Login/Index?message=An error occured");
        }
        private void EndSession()
        {
            Session.Abandon();
        }
    }
}