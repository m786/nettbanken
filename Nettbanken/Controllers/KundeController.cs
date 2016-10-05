using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbanken.Controllers
{
    public class KundeController : Controller
    {
        // Resturnerer standard TestView siden
        public ActionResult TestView()
        {
            return View();
        }
        // Blir brukt når man sender inn registrerings info fra TestView siden
        [HttpPost]
        public ActionResult KundeView(FormCollection innListe)
        {
            String OK;

            String[] a = new String[9];
            a[0] = innListe["bankid"];
            a[1] = innListe["personnr"];
            a[2] = innListe["passord"];
            a[3] = innListe["fornavn"];
            a[4] = innListe["etternavn"];
            a[5] = innListe["adresse"];
            a[6] = innListe["telefonnr"];
            a[7] = innListe["postnr"];
            a[8] = innListe["poststed"];
            OK = Models.DBMetoder.skrivInnKunde(a);

            Response.Write(OK);
            return RedirectToAction("TestView");

        }
    }
}