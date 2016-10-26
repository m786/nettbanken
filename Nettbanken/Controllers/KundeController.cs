using Nettbanken.Models;
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
            // Tester om det er data i databasen, hvis ikke opprettes dummydata
            var db = new Models.DbModell();
            try
            {
                var enDbKunde = db.Kunder.First();
            }
            catch(Exception e)
            {
                DBMetoder.dummyData();  
            }
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
            ModelState.Remove("bankId");
            ModelState.Remove("postNr"); 
            if (ModelState.IsValid)//valider 
            {
                String OK;
                OK = DBMetoder.registrerKunde(kunde); 

                // Hvis OK er tom, så gikk registreringen bra, og går videre
                if (OK == "")
                {
                    return RedirectToAction("hjemmesideView");
                }
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
        public ActionResult kundeLogginnView(Kunde kunde)
        {
            //property som ikke trenger vare med valideringen for innlogging
            ModelState.Remove("fornavn");
            ModelState.Remove("etternavn");
            ModelState.Remove("adresse");
            ModelState.Remove("telefonNr");
            ModelState.Remove("postNr");

            if (ModelState.IsValid)//formValider
            {
                // if-setning sjekker om kunden finnes i databasen
                if (DBMetoder.kundeLogginn(kunde)) 
                {
                    Session["innlogget"] = true;

                    // Initialiserer betalingsListe, trenger en verdi hvis ikke gir det en error ved oppstart
                    var betalingsListe = new List<String[]>();
                    String[] temp = { "initializer" };
                    betalingsListe.Add(temp);

                    String personnr = kunde.personNr;
   
                    Session["personnr"] = kunde.personNr;
                    // Session["kontoNavn"] = kunde.fornavn + " " + kunde.etternavn + ": " + kunde.konto;
                    Session["kontoer"] = DBMetoder.hentKontoer(personnr);
                    Session["tempTabell"] = betalingsListe;

                    return RedirectToAction("hjemmesideView");
                }
            }

            Session["innlogget"] = false;
            return View();
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
            Session["innlogget"] = false;
            Session["personnr"] = null; 
            Session["kontoer"] = null;
            Session["tempTabell"] = null;
            return RedirectToAction("kundeLogginnView");

        }

        // Kaller på metode som henter konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            return DBMetoder.hentKontoInformasjon(kontonavn, personnr);
        }

        // Kaller på metode som henter gitt kontoutskrift
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            return DBMetoder.hentKontoUtskrift(kontonavn, personnr);
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
                string[] endreRad = betalingsListe.ElementAt(Int32.Parse(betalingNr));

                for (int i = 0; i < endreRad.Count(); i++)
                {
                    endreRad[i] = info[i];
                }

                return oppdaterTabell();
            }

            return null;
        }

        // Sletter temporære betalinger
        public String slett(string betalingNr)
        {
            if (ModelState.IsValid)
            {
                var betalingsListe = (List<string[]>)Session["tempTabell"];
                betalingsListe.RemoveAt(Int32.Parse(betalingNr));

                return oppdaterTabell();
            }

            return null;
        }
        
        // Betaler alle temporære betalinger
        public String betal()
        {
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
                    DBMetoder.registrerTransaksjon(t);
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
                       "</tr>";
            }
            tempTable += "</table>";

            return tempTable;
        }
    }
    
   
}