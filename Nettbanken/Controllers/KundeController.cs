﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;
using System.Web.Mvc;
using Nettbanken.BLL;
using Nettbanken.Models;
using Nettbanken.DAL;
using System.Threading;
using System.Windows.Forms;
using System.Timers;

namespace Nettbanken.Controllers
{  
    // KundeController, interaksjonene mellom model og view
    public class KundeController : Controller
    {
        private INettbankBLL _nettbankBLL;
        private Boolean harStartet;
        private new System.Timers.Timer timer1;

        // Konstruktør, relatert til stub/unittesting
        public KundeController()
        {
            _nettbankBLL = new NettbankBLL();
        }

        // Konstruktør, relatert til stub/unittesting
        public KundeController(INettbankBLL stub)
        {
            _nettbankBLL = stub;
        }

        // Returnerer forsiden til Nettbanken
        public ActionResult forsideView()
        {
            // Tester om det er data i databasen, hvis ikke opprettes dummydata
            _nettbankBLL.startsjekk();
            // Start transaksjon sjekkingen.
            transaksjonerStatusSjekking();

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
            if (ModelState.IsValid) //valider 
            {
                // Hvis registrering av kunde OK
                if (_nettbankBLL.registrerKunde(kunde))
                {
                    System.Windows.Forms.MessageBox.Show("Registrering godkjent! Ditt BankID er: " + Session["bankid"]);
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
                // Sjekker om logginn informasjon til admin er gyldig
                if (_nettbankBLL.adminLogginn(admin))
                {
                    // Hvis en logger seg inn som admin, så logges kundekonto ut dersom en kundekonto er innlogget
                    if((Boolean)Session["innlogget"])
                    {
                        Session["innlogget"] = false;
                        Session["personnr"] = null;
                        Session["bankid"] = null;
                        Session["kontoer"] = null;
                        Session["tempTabell"] = null;
                    }
                    Session["innloggetAdmin"] = true;

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

            if (ModelState.IsValid) //Valider
            {
                // Sjekker om innloggingsinformasjoner gyldig
                if (_nettbankBLL.kundeLogginn(kunde)) 
                {
                    // Hvis en logger seg inn som kunde, så logges adminbruker ut dersom en adminbruker er innlogget
                    Session["innloggetAdmin"] = false;
                    Session["innlogget"] = true;

                    // Initialiserer betalingsListe, trenger en verdi hvis ikke gir det en error ved oppstart
                    var betalingsListe = new List<String[]>();
                    String[] temp = { "initializer" };
                    betalingsListe.Add(temp);

                    String personnr = kunde.personNr;
                    
                    // Lagrer nødvendig informasjon inn i sessions
                    Session["personnr"] = kunde.personNr;
                    Session["bankid"] = kunde.bankId;
                    Session["kontoer"] = _nettbankBLL.hentKontoer(personnr);
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
                    List<Kunde> alleKunder = _nettbankBLL.alleKunder();
                    return View(alleKunder);
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
                    // Henter kontoer til gitt kunde
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

                    // Resetter betalingsliste om den har blitt brukt før
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

        // Side som sletter en gitt kunde, brukes av admins
        public ActionResult slettView(string idnr)
        {
            // Kan kun aksesseres dersom man er innlogget som admin
            if (Session["innloggetAdmin"] != null)
            {
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (innlogget)
                {
                    var kunden = _nettbankBLL.finnKunde(idnr);
                    return View(kunden);
                }
                return RedirectToAction("adminLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Siden som endrer en gitt kunde, bruke av admins
        public ActionResult endreView(String idnr)
        {
            // Siden vises kun derso mman er logget inn som admin
            if (Session["innloggetAdmin"] != null)
            {
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (innlogget)
                {
                    Kunde kunden = _nettbankBLL.finnKunde(idnr);
                    return View(kunden);
                }
                return RedirectToAction("adminLogginnView");
            }

            return RedirectToAction("forsideView");
        }

        // Siden brukes når admin submitter endringene som er gjort på kunden 
        [HttpPost]
        public ActionResult endreView(string idnr, Kunde kunde)
        {
            ModelState.Remove("passord");
            ModelState.Remove("bankId");
            ModelState.Remove("personNr");

            if (ModelState.IsValid) //Valider 
            {
                if (_nettbankBLL.endreKunde(idnr, kunde))
                {          
                    return RedirectToAction("adminsideView");
                }
            }

            return View(kunde);       
        }

        // Side som viser informasjon om kunden, brukes av admins
        public ActionResult infoView(String idnr)
        {

            if (Session["innloggetAdmin"] != null)
            {
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (innlogget)
                {
                    var betalingsListe = new List<String[]>();
                    String[] temp = { "initializer" };
                    betalingsListe.Add(temp);

                    String personnr = _nettbankBLL.finnKunde(idnr).personNr;
                    Session["innlogget"] = true;
                    // Lagrer nødvendig informasjon inn i sessions
                    Session["personnr"] = _nettbankBLL.finnKunde(idnr).personNr;
                    Session["bankid"] = _nettbankBLL.finnKunde(idnr).bankId;
                    Session["kontoer"] = _nettbankBLL.hentKontoer(personnr);
                    Session["tempTabell"] = betalingsListe;

                    return RedirectToAction("hjemmesideView");
                }
                return RedirectToAction("adminLogginnView");
            }
            return RedirectToAction("forsideView");
        }

        // Registrerer en ny kunde via admin
        public ActionResult registrerViaAdmin()
        {
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

        // Viewet brukes når admin submitter opprettelsen av den nye kunden
        [HttpPost]
        public ActionResult registrerViaAdmin(Kunde kunde)
        {
            ModelState.Remove("bankId");
            ModelState.Remove("passord");

            if (ModelState.IsValid) //Valider 
            {
                if (_nettbankBLL.registrerNyKunde(kunde))
                {
                    return RedirectToAction("adminsideView");
                }
            }

            return View();
        }

        //Sletter en kunde, brukes av admin
        public ActionResult slettEnKundeFraDB(string idnr) 
        {
            if (Session["innloggetAdmin"] != null)
            {              
                bool innlogget = (bool)Session["innloggetAdmin"];
                if (_nettbankBLL.slettKunde(idnr))
                {
                    return RedirectToAction("adminsideView");
                }
                return RedirectToAction("adminsideView");
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
                Session["bankid"] = null;
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
            return _nettbankBLL.hentKontoInformasjon(kontonavn, personnr);
        }

        // Kaller på metode som henter gitt kontoutskrift
        public String hentKontoUtskrift(String kontonavn, String personnr)
        {
            return _nettbankBLL.hentKontoUtskrift(kontonavn, personnr);
        }

        // Metode som legger til en temporær transaksjon
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
                           "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" +
                           "button"+ " id=\""+i+ "\"onclick=\"knapper('slett',this.id)\"" + ">Slett</button></td>" +
                            "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" +
                            "button" + " id=\""+i+ "\"onclick=\"knapper('endre',this.id)\"" + ">Endre</button></td>" +
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

                // Henter den korrekte betalingen som skal endres
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

                // Sletter den korrekte betalingen
                try
                {
                    betalingsListe.RemoveAt(Int32.Parse(betalingNr));
                    return oppdaterTabell();
                }
                catch (Exception feil)
                {
                    return oppdaterTabell();
                }
            }

            return null;
        }

        // Betaler alle temporære betalinger
        public String betal()
        {
            var betalingsListe = (List<String[]>)Session["tempTabell"];

            if (betalingsListe.Count > 1)
            {
                // Lager transaksjonsobjekt ut av hver temporær transaksjon
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
                    _nettbankBLL.registrerTransaksjon(t);
                }

                // Tømmer lista med temporære betalinger
                betalingsListe.Clear();
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
            if (betalingsListe.Count > 1)
            {
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
                            "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" +
                            "button" + " id=\"" + i + "\"onclick=\"knapper('slett',this.id)\"" + ">Slett</button></td>" +
                                "<td class='col-sm-4' style='background-color:lavenderblush;'><button type=" + "button" +
                                " id=\"" + i + "\"onclick=\"knapper('endre',this.id)\"" + ">Endre</button></td>" +
                           "</tr>";
                }
                tempTable += "</table>";

                return tempTable;
            }
            else
            {
                return "";
            }
        }

        // Start transaksjon sjekkingen automatisk. Denne blir kalt på oppstart av applikasjonen 1 gang
        public void transaksjonerStatusSjekking()
        {
            if (!harStartet)
            {
                // Trenger startes bare 1 gang/appstart
                timer1 = new System.Timers.Timer();
                timer1.Elapsed += new ElapsedEventHandler(sjekkForNyeTransaksjonSomMaaOppdateres);
                timer1.Interval = 10000; // transaksjon interval
                timer1.Enabled = true;
                harStartet = true;
            }
        }

        // Bla gjennom transaksjons tabellen og sjekk for transaksjoner som har status "Ikke Betalt" med datoen idag.
        // Hvis det er noen transaksjon som datoen gaar ut idag, det trekkes og status paa transaksjonen oppdateres.
        // Sjekkingen startes automatisk ved app startup, engang, der etter kjorer automatisk til appen stoppes.
        private void sjekkForNyeTransaksjonSomMaaOppdateres(object sender, EventArgs e)
        {
                _nettbankBLL.startSjekkTransaksjonStatus();
        }
    }
    
   
}

/* Liste over det som trengs å gjøres
 * 
 * 
 * Admin
 *  - Info
 *  - TESTING
 * 
 * Kunde
 * - Bedre regex for transaksjonsfeltene - prioriter admin tingene først. 
 * - nullpointerEX på dato for transaksjonssjekken, sjekk der det er kommenter med "// exception skjer regel messig her", linje 839.
 *   Tingen er at hvis databasen har transaksjoner i transaksjonstabellen så skjer det regelmessige nullpointerExs når vi kjører løsningen
 *    som jeg ikke helt forstår hvorfor skjer, dermed trenger gebi å teste dete.
 * 
 * 
 * 
 * 
 * */