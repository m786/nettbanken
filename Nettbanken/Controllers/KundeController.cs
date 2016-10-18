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
            // Sjekker om session finnes, hvis ikke så settes den
            if (Session["innlogget"] == null)
            {
                Session["innlogget"] = false;
                ViewBag.innlogget = false;
            }
            // ViewBag får session verdien ellers.
            else
            {
                ViewBag.innlogget = (bool)Session["innlogget"];
            }

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

            return View();
        }

        // Kundens innloggingsside
        public ActionResult kundeLogginnView()
        {
            if (Session["innlogget"] != null)
            { 
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    return RedirectToAction("hjemmesideView");
                }
                return View();
            }

            return RedirectToAction("forsideView");
        }
        
        // View som brukes når kunde prøver å logge inn
        [HttpPost]
        public ActionResult kundeLogginnView(Models.Kunde kunde)
        {
            // if-setning sjekker om kunden finnes i databasen
            if (Models.DBMetoder.kundeLogginn(kunde))
            {
                Session["innlogget"] = true;
                ViewBag.innlogget = true;

                String personnr = kunde.personNr;
                return RedirectToAction("hjemmesideView", new { id = personnr});
            }

            Session["innlogget"] = false;
            ViewBag.innlogget = false;
            return View();
        }

        // Hjemmesiden til kunde etter suksessfull innlogging
        public ActionResult hjemmesideView(string id)
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innlogget"] != null)
            {
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    var kontoer = Models.DBMetoder.hentKontoer(id);
                    ViewBag.personnr = id;
                    return View(kontoer);
                }
                return RedirectToAction("kundeLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Side som viser utskrift av transaksjoner
        public ActionResult utskriftView()
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innlogget"] != null)
            {
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    return View();
                }
                return RedirectToAction("kundeLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Siden for utføring av transaksjoner
        public ActionResult transaksjonView()
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innlogget"] != null)
            {
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    return View();
                }
                return RedirectToAction("kundeLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Metode for utlogging
        public ActionResult loggUt()
        {
            Session["innlogget"] = false;
            return RedirectToAction("kundeLogginnView");

        }

        public String hentTransaksjoner(String kontonavn, String personnr)
        {
            return Models.DBMetoder.hentTransaksjoner(kontonavn, personnr);
        }

    }
}