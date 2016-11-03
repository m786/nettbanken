
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Nettbanken.BLL;
using Nettbanken.Models;

namespace Nettbanken.Controllers
{
    
    // KundeController, der alle metodene som kunden utfører/trenger blir plassert. 
    public class KundeController : Controller
    {
        // Returnerer forsiden til Nettbanken
        public ActionResult forsideView()
        {
            // Tester om det er data i databasen, hvis ikke opprettes dummydata
            var nettbankenBLL = new NettbankBLL();
            nettbankenBLL.startsjekk();
            
            // Kunde session
            // Sjekker om session finnes, hvis ikke så settes den
            if (Session["innlogget"] == null)
            {
                Session["innlogget"] = false;
            }

            // Admin Session
            // Sjekker om session finnes, hvis ikke så settes den
            if (Session["innloggetAdmin"] == null)
            {
                Session["innloggetAdmin"] = false;
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
        public ActionResult kundeRegistreringView(Kunde kunde)
        {
            ModelState.Remove("bankId");
            if (ModelState.IsValid)//valider 
            {
                var nettbankBLL = new NettbankBLL();

                // Hvis OK er tom, så gikk registreringen bra, og går videre
                if (nettbankBLL.registrerKunde(kunde))
                {
                    return RedirectToAction("kundeLogginnView");
                }
            }
            return View();
        }

        // Innloggingsside for admins
        public ActionResult adminLogginnView()
        {
            if (Session["innloggetAdmin"] != null)
            {
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (innlogget)
                {
                    return RedirectToAction("adminsideView");
                }
                return View();
            }

            return RedirectToAction("forsideView");
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

        // Viewet som brukes når admin prøver å logge seg inn
        [HttpPost]
        public ActionResult adminLogginnView(Admin admin)
        {
            // Property som ikke trenger vare med valideringen for innlogging
            ModelState.Remove("fornavn");
            ModelState.Remove("etternavn");
            ModelState.Remove("adresse");
            ModelState.Remove("telefonNr");
            ModelState.Remove("postNr");
            ModelState.Remove("poststed");

            if (ModelState.IsValid)
            {
                var nettbankBLL = new NettbankBLL();
                if (nettbankBLL.adminLogginn(admin))
                {
                    // Hvis en logger seg inn som admin, så logges kundekonto ut dersom en kundekonto er innlogget
                    Session["innloggetAdmin"] = true;
                    Session["innlogget"] = false;
                    return RedirectToAction("adminsideView");
                }
            }

            Session["innloggetAdmin"] = false;
            return View();
        }

        // View som brukes når kunde prøver å logge inn
        [HttpPost]
        public ActionResult kundeLogginnView(Kunde kunde)
        {
            // Property som ikke trenger vare med valideringen for innlogging
            ModelState.Remove("fornavn");
            ModelState.Remove("etternavn");
            ModelState.Remove("adresse");
            ModelState.Remove("telefonNr");
            ModelState.Remove("postNr");
            ModelState.Remove("poststed");

            if (ModelState.IsValid)//formValider
            {
                var nettbankBLL = new NettbankBLL();
                // if-setning sjekker om kunden finnes i databasen
                if (nettbankBLL.kundeLogginn(kunde)) 
                {
                    // Hvis en logger seg inn som kunde, så logges adminbruker ut dersom en adminbruker er innlogget
                    Session["innloggetAdmin"] = false;
                    Session["innlogget"] = true;

                    // Initialiserer betalingsListe, trenger en verdi hvis ikke gir det en error ved oppstart
                    var betalingsListe = new List<String[]>();
                    String[] temp = { "initializer" };
                    betalingsListe.Add(temp);

                    String personnr = kunde.personNr;
   
                    Session["personnr"] = kunde.personNr;
                    // Session["kontoNavn"] = kunde.fornavn + " " + kunde.etternavn + ": " + kunde.konto;
                    Session["kontoer"] = nettbankBLL.hentKontoer(personnr);
                    Session["tempTabell"] = betalingsListe;

                    return RedirectToAction("hjemmesideView");
                }
            }

            Session["innlogget"] = false;
            return View();
        }

        // Hjemmesiden til admins
        public ActionResult adminsideView()
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innloggetAdmin"] != null)
            {
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (innlogget)
                {
                    return View();
                }
                return RedirectToAction("adminLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Hjemmesiden til kunde etter suksessfull innlogging
        public ActionResult hjemmesideView()
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innlogget"] != null)
            {
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    // Henter kontoer til gitt kunde (id)
                    var kontoer = (List<String>)Session["kontoer"];
                    ViewBag.personnr = (String)Session["personnr"];

                    return View(kontoer);
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
                    ViewBag.personnr = (String)Session["personnr"];
                    ViewBag.kontoer = (List<String>)Session["kontoer"];

                    // Resetter betalinhsliste om den har blitt brukt før
                    var betalingsListe = (List<String[]>)Session["tempTabell"];
                    if (betalingsListe.Count > 1)
                    {
                        betalingsListe.Clear(); //clear transaction buffer
                        String[] temp = { "initializer" };
                        betalingsListe.Add(temp);
                    }

                    Session["tempTabell"] = betalingsListe;

                    return View();
                }
                return RedirectToAction("kundeLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Metode for utlogging
        public ActionResult loggUt()
        {
            // Kun en kunde/admin kan være innlogget til enhver tid, dermed sjekker vi hvem som er innlogget
            // basert på hvem, så utføres utlike utloggingsactions
            if ((Boolean)Session["innlogget"])
            {
                Session["innlogget"] = false;
                Session["personnr"] = null;
                Session["kontoer"] = null;
                Session["tempTabell"] = null;
            }
            else
            {
                Session["innloggetAdmin"] = false;
            }

            return RedirectToAction("forsideView");
        }

        // Kaller på metode som henter konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            var nettbankBLL = new NettbankBLL();
            return nettbankBLL.hentKontoInformasjon(kontonavn, personnr);
        }

        // Kaller på metode som henter gitt kontoutskrift
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            var nettbankBLL = new NettbankBLL();
            return nettbankBLL.hentKontoUtskrift(kontonavn, personnr);
        }

        // Metode som legger til en transaksjon temporert
        public String tempTransaksjon(String[] infoliste)
        {
            if (ModelState.IsValid)
            {
                // Session ble intialisert ved innlogging
                // Brukes for å bevare lista med temp betalinger (Huske-mekanisme)
                var betalingsListe = (List<string[]>)Session["tempTabell"];
                betalingsListe.Add(infoliste);
                Session["tempTabell"] = betalingsListe;

                String tempTable = "<table>" + "<tr>" +
                     "<th class='col-sm-4' style='background-color:lavenderblush;'>Betaling Nummer</th>" +
                    "<th class='col-sm-4' style='background-color:lavenderblush;'>Betalings Dato</th>" +
                    "<th class='col-sm-4' style='background-color:lavender;'>Mottaker</th>" +
                    "<th class='col-sm-4' style='background-color:lavenderblush;'>Beløp</th>" +
                    "</tr>";
                for (var i = 1; i < betalingsListe.Count; i++)
                {
                    String[] tmp = betalingsListe.ElementAt(i);
                    tempTable +=
                           "<tr>" +
                           "<td class='col-sm-4' style='background-color:lavenderblush;'>" + i + "</td>" +
                           "<td class='col-sm-4' style='background-color:lavenderblush;'>" + tmp[4] + "</td>" +
                           "<td class='col-sm-4' style='background-color:lavender;'>" + tmp[1] + "</td>" +
                           "<td class='col-sm-4' style='background-color:lavenderblush;'>" + tmp[2] + "</td>" +

                           //finn en maate aa lytte paa disse knappene!!! blir jquery!  dette   gaar ann men vi trenger unike id for hver knapper nummerere dem!!
                           "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" + "button"+ " id=\""+i+ "\"onclick=\"knapper('slett',this.id)\"" + ">Slett</button></td>" +
                            "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" + "button" + " id=\""+i+ "\"onclick=\"knapper('endre',this.id)\"" + ">Endre</button></td>" +
                           "</tr>";
                }
                tempTable += "</table>";
                return tempTable;
            }

            return null;
        }

        // Metode som endrer på en temporær transaksjon
        public String endre(string betalingNr, string[] info)
        {
            if (ModelState.IsValid)
            {
                var betalingsListe = (List<string[]>)Session["tempTabell"];

                // Sjekker om oppgitte endringsnummer tilhører en temporær betaling
                try
                {
                    String[] endreRad = betalingsListe.ElementAt(Int32.Parse(betalingNr));
                    for (int i = 0; i < endreRad.Count(); i++)
                    {
                        endreRad[i] = info[i];
                    }

                    return oppdaterTabell();
                }
                catch (Exception feil)
                {
                    return oppdaterTabell() + "<div><b>Oppgitte endringsnummer finnes ikke!</b></div>"; 
                }
            }

            return null;
        }

        // Sletter temporære betalinger
        public String slett(string betalingNr)
        {
            if (ModelState.IsValid)
            {
                var betalingsListe = (List<string[]>)Session["tempTabell"];

                // Sjekker om oppgitte slettenummer tilhører en temporær betaling
                try
                {
                    betalingsListe.RemoveAt(Int32.Parse(betalingNr));
                    return oppdaterTabell();
                }
                catch (Exception feil)
                {
                    return oppdaterTabell() + "<div><b>Oppgitte slettenummer finnes ikke!</b></div>";
                }
            }

            return null;
        }
        
        // Betaler alle temporære betalinger
        public String betal()
        {
            var nettbankBLL = new NettbankBLL();
            var betalingsListe = (List<String[]>)Session["tempTabell"];
            if (betalingsListe.Count > 1)
            {
                for (int i = 1; i < betalingsListe.Count(); i++)
                {
                    Transaksjon t = new Transaksjon();
                    string[] rad = betalingsListe.ElementAt(i);
                    t.fraKonto = rad[0];
                    t.tilKonto = rad[1];
                    t.saldoUt = Int32.Parse(rad[2]);
                    t.KID = rad[3];
                    t.dato = rad[4];
                    t.melding = rad[5];
                    nettbankBLL.registrerTransaksjon(t);
                }

                betalingsListe.Clear(); //clear transaction buffer
                String[] temp = { "initializer" };
                betalingsListe.Add(temp);

                return "<div><b>Betalinger fullført.</b> <br/> " +
                    "Legg til nye betalinger eller gå tilbake til hjemmesiden for å se tillagte betalinger.</div>";
            }

            return "<div><b>Du må legge til betalinger før utførelse av betalinger!</b></div>";
        }

        // Oppdaterer tabellen som viser alle temporære betalinger
        public String oppdaterTabell()
        {
            var betalingsListe = (List<string[]>)Session["tempTabell"];
            String tempTable = "<table>" + "<tr>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Betaling Nummer</th>" +
               "<th class='col-sm-4' style='background-color:lavenderblush;'>Betalings Dato</th>" +
               "<th class='col-sm-4' style='background-color:lavender;'>Mottaker</th>" +
               "<th class='col-sm-4' style='background-color:lavenderblush;'>Beløp</th>" +
               "</tr>";
            for (var i = 1; i < betalingsListe.Count; i++)
            {
                String[] tmp = betalingsListe.ElementAt(i);
                tempTable +=
                       "<tr>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" + i + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" + tmp[4] + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavender;'>" + tmp[1] + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" + tmp[2] + "</td>" +

                        "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" + "button" + " id=\"" + i + "\"onclick=\"knapper('slett',this.id)\"" + ">Slett</button></td>" +
                            "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" + "button" + " id=\"" + i + "\"onclick=\"knapper('endre',this.id)\"" + ">Endre</button></td>" +
                       "</tr>";
            }
            tempTable += "</table>";

            return tempTable;
        }
    }
    
   
}