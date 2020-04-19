using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;

namespace API.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        private ILog _iLog;
        public ExceptionFilter()
        {
            _iLog = Log.GetInstance;
        }
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var msg = _iLog.CaughtExceptions(actionExecutedContext.Exception, actionExecutedContext.ActionContext.ControllerContext.Controller.ToString());
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(msg)
            };
            actionExecutedContext.Response = response;
        }
    }
}