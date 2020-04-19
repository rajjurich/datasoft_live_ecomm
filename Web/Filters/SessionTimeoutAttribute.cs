using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Filters
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        private ILog _iLog;

        public SessionTimeoutAttribute()
        {
            _iLog = Log.GetInstance;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["accessToken"] == null)
            {
                _iLog.Logger("access token null", Enumerations.LogType.Exception);
                filterContext.Result = new RedirectResult("~/Login/Index?message=Session invalid");
                return;
            }
        }
    }
}