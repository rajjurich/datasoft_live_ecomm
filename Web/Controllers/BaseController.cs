using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        private ILog _iLog;

        public BaseController()
        {
            _iLog = Log.GetInstance;
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            var msg = _iLog.CaughtExceptions(filterContext.Exception, filterContext.Controller.ToString());
        }        
    }
}