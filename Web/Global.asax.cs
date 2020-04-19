using Autofac;
using Autofac.Integration.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Web.Common;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<Config>().As<IConfig>().InstancePerLifetimeScope();
            builder.Register(c => new HttpClient()).As<HttpClient>().SingleInstance();
            var dataAccess = Assembly.GetExecutingAssembly();
            var referencedAssemblyName = dataAccess.GetReferencedAssemblies()
                                                  .Where(n => n.Name.Equals("Client"))
                                                  .FirstOrDefault();
            Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);

            builder.RegisterAssemblyTypes(referencedAssembly)
                   .Where(t => t.Name.EndsWith("Client"))
                   .AsImplementedInterfaces();
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
