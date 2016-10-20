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
            var db = new DbModell();
            var count = db.Kunder.Count();
            if (count == 0) { //da er det ikke noe i db
                    KundeController.dummyData(); // opprett dummy data 
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

                String personnr = kunde.personNr;
                Session["personnr"] = kunde.personNr;
                Session["kontoer"] = Models.DBMetoder.hentKontoer(personnr);

                return RedirectToAction("hjemmesideView");
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

        ///////////////////////////DummyData////////////////////////////////
        public static void dummyData() 
        {
            string[] fornavn = new string[] { "Per", "Ola", "Maria", "Marius", "Helen", "Brage", "Najmi" };
            string[] etternavn = new string[] { "Bakke", "Hansen", "Dilora", "Kalle", "Desta", "Petter", "Suda" };
            string[] poststed = new string[] { "Oslo", "Bergen", "Stavanger", "Kristia", "Haugesund", "Hammer", "Oslo" };
            string[] adresse = new string[] { "Helba 2", "Femti 21", "Hokk 34", "Turn 12", "Kort 22", "Malibu 2", "Halv Life 3" };

            int pernr = 011189211, tlf = 555555, konNr = 12345, postNr = 6789;
            Models.Kunde k;
            Models.Poststed p;
            Models.Konto s;

            for (var i = 0; i < fornavn.Length; i++)
            {
                pernr += i;
                tlf += 1;
                konNr += 1;
                postNr += 1;

                 k = new Models.Kunde();
                 p = new Models.Poststed();
                 s = new Models.Konto();

                p.poststed = poststed[i];
                k.personNr = pernr + "";
                k.passord = "asdfasdf";
                k.fornavn = fornavn[i];
                k.etternavn = etternavn[i];
                k.adresse = adresse[i];
                k.telefonNr = tlf + "";
                k.postNr = p.postNr = postNr + "";
                k.poststed = p;
                DBMetoder.registrerKunde(k);
                s.kontoNr = konNr + "";
                s.saldo = 500;
                s.kontoNavn = k.fornavn + " " + k.etternavn;
                s.personNr = k.personNr;
                DBMetoder.registrerNyKonto(s);

                if (i == fornavn.Length-1)
                {
                    pernr += i;
                    //2 ekstra kontoer for personNR 1 og 1 ekstra konto for person nr 2!
                    Models.Konto e = new Models.Konto();
                    e.kontoNr = 123456 + "";
                    e.saldo = 50;
                    e.kontoNavn = "Per" + " " + "Bakke"; 
                    e.personNr = 11189211 + "";
                    DBMetoder.registrerNyKonto(e);

                    pernr += i;
                    Models.Konto f = new Models.Konto();
                    f.kontoNr = 123457 + "";
                    f.saldo = 400;
                    f.kontoNavn = "Per" + " " + "Bakke";
                    f.personNr = 11189211 + "";

                    DBMetoder.registrerNyKonto(f);

                    pernr += i;
                    Models.Konto g = new Models.Konto();
                    g.kontoNr = 2221 + "";
                    g.saldo = 50;
                    g.kontoNavn = "Ola" + " " + "Hansen"; 
                    g.personNr = 11189212 + "";
                    DBMetoder.registrerNyKonto(g);
                }
            }
        }
        ///////////////////////////DummyData////////////////////////////////

    }
}