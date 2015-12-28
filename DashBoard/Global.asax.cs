﻿using DashBoard.Hubs;
using StockModel.Master;
using StockServices.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DashBoard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
            List<Exchange> exchanges = Enum.GetValues(typeof(Exchange)).OfType<Exchange>().ToList();

            InMemoryObjects.LoadInMemoryObjects(exchanges);
        }

        protected void Application_End()
        {
            
        }
        
    }
}
