using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bazinam.web.Areas.Admin.Controllers
{
    public partial class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        public virtual ActionResult Index()
        {
            return View();
        }
    }
}