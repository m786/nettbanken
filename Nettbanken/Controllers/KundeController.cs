using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbanken.Controllers
{

    // KundeController, der alle metodene som kunden utfører/trenger blir plassert. 
    public class KundeController : Controller
    {
        // Returnerer forsiden til Nettbanken
        public ActionResult forsideView()
        {
            return View();
        }

        // Kundens innloggingsside
        public ActionResult kundeLogginnView()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult kundeLogginnView(FormCollection info)
        {
            String[] a = new String[3];
            a[0] = info["bankid"];
            a[1] = info["personnr"];
            a[2] = info["passord"];

            Models.Kunde kunde = Models.DBMetoder.kundeLogginn(a);

            if (kunde.bankId.Equals(a[0]) && kunde.personNr.Equals(a[1]) && kunde.passord.Equals(a[2]))
            {
                return hjemmesideView();
            }
            return View();

        }

        public ActionResult hjemmesideView()
        {
            return View();
        }
        
    }
}