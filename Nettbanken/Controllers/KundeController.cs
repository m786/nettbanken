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
            var db = new Models.DbModell();
          try
            {
                var enDbKunde = db.Kunder.First();
            }
            catch(Exception e)
            {
                DBMetoder.dummyData(); // opprett dummy data  
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
                OK = Models.DBMetoder.registrerKunde(kunde,true); 

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
        public ActionResult kundeLogginnView(Models.Kunde kunde)
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
                if (Models.DBMetoder.kundeLogginn(kunde)) 
                {
                    Session["innlogget"] = true;

                    String personnr = kunde.personNr;
                    // Initialiserer betalingsListe, trenger en verdi hvis ikke gir det en error ved oppstart
                    var betalingsListe = new List<String[]>();
                    String[] temp = { "initializer" };
                    betalingsListe.Add(temp);

                    Session["personnr"] = kunde.personNr;
                   // Session["kontoNavn"] = kunde.fornavn + " " + kunde.etternavn + ": " + kunde.konto;
                    Session["kontoer"] = Models.DBMetoder.hentKontoer(personnr);
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

        // Siden for utføring av transaksjoner/////////////////////////////////////////////////////////////////////////////////////
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
                    return View();
                }
                return RedirectToAction("kundeLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        [HttpPost]
        public ActionResult transaksjonView(Models.Transaksjon transaksjon)
        {
            // Siden kan kun vises dersom man er innlogget
            if (Session["innlogget"] != null)
            {
                bool innlogget = (bool)Session["innlogget"];
                if (innlogget)
                {
                    var personnr = (String)Session["personnr"];
                    var kontoer = (List<String>)Session["kontoer"];

                    ViewBag.kontoer = kontoer;
                    ViewBag.personnr = personnr;
                    Models.Transaksjon t = Models.DBMetoder.registrerTransaksjon(personnr, transaksjon);
                    return RedirectToAction("hjemmesideView");
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
            return RedirectToAction("kundeLogginnView");

        }

        // Kaller på metode som henter konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            return Models.DBMetoder.hentKontoInformasjon(kontonavn, personnr);
        }

        // Kaller på metode som henter gitt kontoutskrift
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            return Models.DBMetoder.hentKontoUtskrift(kontonavn, personnr);
        }

       // [HttpPost]
        public String tempTr(String[] infoliste)
        {
            // Session ble intialisert ved innlogging
            // Brukes for å bevare lista med temp betalinger (Huske-mekanisme)
            var betalingerListe = (List<string[]>)Session["tempTabell"];
            betalingerListe.Add(infoliste);
            Session["tempTabell"] = betalingerListe;
            int betalingNr = 0;

            String tempTable = "<table>" + "<tr>" +
                 "<th class='col-sm-4' style='background-color:lavenderblush;'>Betaling Nummer</th>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Betalings Dato</th>" +
                "<th class='col-sm-4' style='background-color:lavender;'>Mottaker</th>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Beløp</th>" + 
                "</tr>";
            for (var i = 1; i< betalingerListe.Count;i++) 
            {
                String[] tmp = betalingerListe.ElementAt(i); 
                tempTable +=
                       "<tr>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" + i + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" +tmp[4] + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavender;'>" + tmp[1] + "</td>" +
                       "<td class='col-sm-4' style='background-color:lavenderblush;'>" + tmp[2]+ "</td>" +
                       "</tr>";
            }
            tempTable += "</table>";
            return tempTable; 
        }

        public String slett(string betalingNr)
        {
            var betalingerListe = (List<string[]>)Session["tempTabell"];
            betalingerListe.RemoveAt(Int32.Parse(betalingNr));
            return oppdaterTabell();
        }
        
        public String endre(string betalingNr,string[] info)
        {
            var betalingerListe = (List<string[]>)Session["tempTabell"];
            string[] endreRad = betalingerListe.ElementAt(Int32.Parse(betalingNr));

            for(int i=0;i<endreRad.Count()-1;i++)
            {
                endreRad[i] = info[i]; 
            }
            return oppdaterTabell(); 

        }

        public String betal()
        {
            DBMetoder.registrerBetaling((List<string[]>)Session["tempTabell"], (String)Session["personnr"]);
            var betalingerListe = (List<string[]>)Session["tempTabell"];
            betalingerListe.Clear();//clear transaction buffer
            return "<div>Det er ingen betaling som er lagt til</div>";
        }

        public String oppdaterTabell()
        {
            var betalingerListe = (List<string[]>)Session["tempTabell"];
            String tempTable = "<table>" + "<tr>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Betaling Nummer</th>" +
               "<th class='col-sm-4' style='background-color:lavenderblush;'>Betalings Dato</th>" +
               "<th class='col-sm-4' style='background-color:lavender;'>Mottaker</th>" +
               "<th class='col-sm-4' style='background-color:lavenderblush;'>Beløp</th>" +
               "</tr>";
            for (var i = 1; i < betalingerListe.Count; i++)
            {
                String[] tmp = betalingerListe.ElementAt(i);
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