using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using HarnishBalanceSheet.Controllers;
using HarnishBalanceSheet.DataAccess;

namespace HarnishBalanceSheet.App_Start
{
    public class CustomControllerFactory : IControllerFactory
    {
        /// <summary>
        /// Creates a controller. 
        /// </summary>
        /// <param name="requestContext"> HTTP request context. </param>
        /// <param name="controllerName"> Controller name. </param>
        /// <returns> A controller. </returns>
        public IController CreateController(RequestContext requestContext, string controllerName)
        {
            Trace.TraceInformation("Entering CustomControllerFactory.CreateController.");

            AzureRepository repository = new AzureRepository();
            var controller = new BalanceSheetController(repository);

            Trace.TraceInformation("Exiting CustomControllerFactory.CreateController.");
            return controller;
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        /// <summary>
        /// Disposes the controller. 
        /// </summary>
        /// <param name="controller"> Controller to dispose. </param>
        public void ReleaseController(IController controller)
        {
            Trace.TraceInformation("Entering CustomControllerFactory.ReleaseController.");

            IDisposable disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            Trace.TraceInformation("Exiting CustomControllerFactory.ReleaseController.");
        }
    }
}