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

        // Side for registrering av kunde
        public ActionResult kundeRegistreringView()
        {
            return View();
        }

        // View som brukes når kunde registrer seg
        // Pga oppsett i view, så returnerer det et objekt av kunde.
        [HttpPost]
        public ActionResult kundeRegistreringView(Models.Kunde kunde)
        {
            String OK;
            
            OK = Models.DBMetoder.registrerKunde(kunde);
            
            // Hvis OK er tom, så gikk registreringen bra, og går videre
            if (OK == "")
            {
                return RedirectToAction("hjemmesideView");
            }

            //Response.Write(OK);
            return View();
        }

        // Kundens innloggingsside
        public ActionResult kundeLogginnView()
        {
            return View();
        }
        
        // View som brukes når kunde prøver å logge inn
        [HttpPost]
        public ActionResult kundeLogginnView(Models.Kunde kunde)
        {
            return View();
        }

        // Hjemmesiden til kunde etter suksessfull innlogging
        public ActionResult hjemmesideView()
        {
            return View();
        }
        
    }
}