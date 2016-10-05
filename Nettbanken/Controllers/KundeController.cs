using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbanken.Controllers
{
    public class KundeController : Controller
    {
        public ActionResult TestView()
        {
            List<Models.Kunde> kunder = 
            String ret = Models.DBMetoder.skrivInnKunde();
            ViewData["Melding"] = ret;
            return View();
        }
    }
}