using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using Domain.Entities;
using License;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace API.Filters
{
    public class ApplicationConfigurationCheck : ActionFilterAttribute
    {
        public IApplicationConfigurationService _applicationConfigurationService { get; set; }
        public override async Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            _applicationConfigurationService = actionContext.Request.GetDependencyScope().GetService(typeof(IApplicationConfigurationService)) as IApplicationConfigurationService;
            
            if (Check.productExpiryDate < DateTime.Now)
            {
                throw new Exception("Your license has expired.");
            }
            var x = await _applicationConfigurationService.Get();
            if (!(x.SystemInformation.Equals(Check.systemInfo)))
            {
                throw new Exception("Your license is invalid.");
            }
        }

    }
}