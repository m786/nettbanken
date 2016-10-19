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

        ///////////////////////////DummyData////////////////////////////////
        public static void dummyData() 
        {
            string[] fornavn = new string[] { "Per", "Ola", "Maria", "Marius", "Helen", "Brage", "Najmi" };
            string[] etternavn = new string[] { "Bakke", "Hansen", "Dilora", "Kalle", "Desta", "Petter", "Suda" };
            string[] poststed = new string[] { "Oslo", "Bergen", "Stavanger", "Kristia", "Haugesund", "Hammer", "Oslo" };
            string[] adresse = new string[] { "Helba 2", "Femti 21", "Hokk 34", "Turn 12", "Kort 22", "Malibu 2", "Halv Life 3" };

            int pernr = 011189211, tlf = 555555, konNr = 12345, postNr = 6789;


            for (var i = 0; i < fornavn.Length; i++)
            {
                pernr += i;
                tlf += 1;
                konNr += 1;
                postNr += 1;

                Models.Kunde k = new Models.Kunde();
                Models.Poststed p = new Models.Poststed();
                Models.Konto s = new Models.Konto();

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
            }

        }
        ///////////////////////////DummyData////////////////////////////////

    }
}