using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nettbanken.Controllers
{
    public class KundeController : Controller
    {
        // Resturnerer standard TestView side
        public ActionResult TestView()
        {
            return View();
        }
        // Returnerer standard AdminView side
        public ActionResult AdminView()
        {
            return View();
        }
        // Blir brukt når man sender inn registreringsinfo fra AdminView
        [HttpPost]
        public ActionResult AdminView(FormCollection innListe)
        {
            String OK; // Innsettingsstatus

            String[] a = new String[9];
            a[0] = innListe["adminid"];
            a[1] = innListe["passord"];
            a[2] = innListe["fornavn"];
            a[3] = innListe["etternavn"];
            a[4] = innListe["adresse"];
            a[5] = innListe["telefonnr"];
            a[6] = innListe["postnr"];
            a[7] = innListe["poststed"];
            OK = Models.DBMetoder.skrivInnAdmin(a);

            Response.Write(OK);
            return View();
        }
        // Returnerer standard KundeView side
        public ActionResult KundeView()
        {
            return View();
        }
        // Blir brukt når man sender inn registreringsinfo fra KundeView
        [HttpPost]
        public ActionResult KundeView(FormCollection innListe)
        {
            String OK; // Innsettingsstatus

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
            return View();
        }
        // Returnerer standard TransaksjonView side
        public ActionResult TransaksjonView()
        {
            return View();
        }
        // Blir brukt når man sender inn transaksjonsinfo fra TransaksjonView
        [HttpPost]
        public ActionResult TransaksjonView(FormCollection innListe)
        {
            String OK; // Innsettingsstatus

            String[] a = new String[8];
            a[0] = innListe["status"];
            a[1] = innListe["saldoinn"];
            a[2] = innListe["saldout"];
            a[3] = innListe["dato"];
            a[4] = innListe["kid"];
            a[5] = innListe["frakonto"];
            a[6] = innListe["tilkonto"];
            a[7] = innListe["melding"];
            OK = Models.DBMetoder.skrivInnTransaksjon(a);

            Response.Write(OK);
            return View();
        }
    }
}