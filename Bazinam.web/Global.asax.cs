using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Bazinam.DataAccessLayer;
using Bazinam.web.App_Start;

namespace Bazinam.web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext,
                    Bazinam.DataAccessLayer.Migrations.Configuration>());
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
    }
}