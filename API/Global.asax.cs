using API.Models;
using Autofac;
using Autofac.Integration.WebApi;
using Domain.Core;
using Domain.Services;
using Logger;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private ILog _Ilog;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AutoFacBuilder();
            var check = ConfigurationManager.AppSettings["DBInitialize"];
            var isEnabled = check == "true" ? true : false;
            if (isEnabled)
            {
                Database.SetInitializer(new MigrateDatabaseToLatestVersion<EntitiesContext, Domain.Migrations.Configuration>());
                EntitiesContext ec = new EntitiesContext();
                ec.Database.Initialize(true);

                Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, API.Migrations.Configuration>());
                ApplicationDbContext ac = new ApplicationDbContext();
                ac.Database.Initialize(true);
            }           
        }
        protected void Application_Error()
        {
            Exception ex = Server.GetLastError();
            _Ilog = Log.GetInstance;
            _Ilog.Logger(ex.Message, Enumerations.LogType.Exception);
            Server.ClearError();
        }
        private static void AutoFacBuilder()
        {
            var builder = new ContainerBuilder();
            
            var config = GlobalConfiguration.Configuration;

            var dataAccess = Assembly.GetExecutingAssembly();
            
            builder.RegisterApiControllers(dataAccess);
            
            builder.RegisterType<EntitiesContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<ApplicationConfigurationService>().As<IApplicationConfigurationService>().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(EntityRepository<>))
                   .As(typeof(IEntityRepository<>))
                   .InstancePerLifetimeScope();
            
            var referencedAssemblyName = dataAccess.GetReferencedAssemblies()
                                                   .Where(n => n.Name.Equals("Domain"))
                                                   .FirstOrDefault();
            Assembly referencedAssembly = Assembly.Load(referencedAssemblyName);
            
            builder.RegisterAssemblyTypes(referencedAssembly)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces();            
           
            var container = builder.Build();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
