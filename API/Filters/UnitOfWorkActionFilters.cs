using Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using Logger;

namespace API.Filters
{
    public class UnitOfWorkActionFilters : ActionFilterAttribute
    {
        public IUnitOfWork UnitOfWork { get; set; }
        private ILog _iLog;
        public UnitOfWorkActionFilters()
        {
            _iLog = Log.GetInstance;
        }         
        public override void OnActionExecuting(HttpActionContext actionContext)
        {            
            UnitOfWork = actionContext.Request.GetDependencyScope().GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            //_iLog.Logger("Transaction Begin - " + actionContext.ControllerContext.Controller.ToString(), Enumerations.LogType.Transaction);
            UnitOfWork.BeginTransaction();
        }
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            UnitOfWork = actionExecutedContext.Request.GetDependencyScope().GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            if (actionExecutedContext.Exception == null)
            {
                // commit if no exceptions
                //_iLog.Logger("Transaction Committed - " + actionExecutedContext.ActionContext.ControllerContext.Controller.ToString(), Enumerations.LogType.Transaction);
                UnitOfWork.Commit();
            }
            else
            {
                // rollback if exception
                //_iLog.Logger("Transaction Rollback - " + actionExecutedContext.ActionContext.ControllerContext.Controller.ToString(), Enumerations.LogType.Transaction);
                //_iLog = Log.GetInstance;
                //_iLog.CaughtExceptions(actionExecutedContext.Exception, actionExecutedContext.ActionContext.ControllerContext.Controller.ToString());
                UnitOfWork.Rollback();
            }
            //_iLog.Logger("Transaction Ends - " + actionExecutedContext.ActionContext.ControllerContext.Controller.ToString(), Enumerations.LogType.Transaction);
        }
    }
}