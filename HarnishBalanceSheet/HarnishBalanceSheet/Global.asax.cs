using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HarnishBalanceSheet.App_Start;

namespace HarnishBalanceSheet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
           // new Startup().Configuration()
            RegisterCustomControllerFactory();            
        }

        /// <summary>
        /// Registers the custom controller factory. 
        /// </summary>
        private void RegisterCustomControllerFactory()
        {
            Trace.TraceInformation("Entering MvcApplication.RegisterCustomControllerFactory.");

            IControllerFactory factory = new CustomControllerFactory();
            ControllerBuilder.Current.SetControllerFactory(factory);

            Trace.TraceInformation("Exiting MvcApplication.RegisterCustomControllerFactory.");
        }
    }
}
