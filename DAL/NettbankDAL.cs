
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbanken.Models;
using System.Security.Cryptography;
using System.Text;
using Nettbanken.DAL;
using System.Net.Http;

namespace Nettbanken.DAL
{
    // Klasse for alle metoder som interagerer med databasen, aka DAL
    public class NettbankDAL : INettbankDAL
    {
        // ---------------------------------------------------------------------------------------
        // Admin Metoder

        // Innloggingsmetode for admins
        public Boolean adminLogginn(Admin admin)
        {
            using (var db = new DBContext())
            {
                // Sjekker oppgitte adminID mot databasen om admin finnes
                AdminDB fantAdmin = db.Admins.FirstOrDefault
                    (a => a.adminId == admin.adminId);

                if (fantAdmin != null)
                {
                    // Dersom admin finnes så sjekker vi om oppgitte passord er korrekt
                    String passord = krypterPassord(admin.passord, fantAdmin.salt);
                    if (passord == fantAdmin.passord)
                    {
                        return true;
                    }
                }

                return false;
            }
        }


        // Henter alle kunder for admin
        public List<Kunde> alleKunder()
        {
            using (var db = new DBContext())
            {
                List<Kunde> alleKunder = db.Kunder.Select(k => new Kunde()
                {
                    personNr = k.personNr,
                    fornavn = k.fornavn,
                    etternavn = k.etternavn,
                    adresse = k.adresse,
                    postNr = k.postNr,
                    poststed = k.poststed.poststed,
                    telefonNr = k.telefonNr
                }).ToList();

                return alleKunder;
            }
        }

        // Registrerer en ny kunde
        public Boolean registrerNyKunde(Kunde kunde)
        {
            // Oppretter Database connection
            using (var db = new DBContext())
            {
                int bid = db.Kunder.Count();
                bid += 1;
                String bankId = bid.ToString();
                string[] kundeInfo = { kunde.fornavn, kunde.etternavn, kunde.personNr };
                byte[] salt = genererSalt();

                // Sjekker om postnr og poststed allerede finnes
                bool finnes = db.Poststeder.Any(p => p.postNr == kunde.postNr);

                // Om postnr og poststed finnes så opprettes en ny kunde 
                // uten noe i Poststed klasse-attributett til kunden
                if (finnes)
                {
                    var nyKunde = new KundeDB
                    {
                        bankId = bankId,
                        personNr = kunde.personNr,
                        passord = krypterPassord(lagPassord(), salt),
                        salt = salt,
                        fornavn = kunde.fornavn,
                        etternavn = kunde.etternavn,
                        adresse = kunde.adresse,
                        telefonNr = kunde.telefonNr,
                        postNr = kunde.postNr
                    };

                    try
                    {
                        db.Kunder.Add(nyKunde);
                        db.SaveChanges();
                        opprettStandardkonto(kundeInfo);
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under registrering av kunde! - "
                            + feil.Message + " - " + feil.InnerException, false);
                        return false;
                    }

                }
                // Postnr og poststed finnes ikke, 
                // legger inne kunden og oppretter en ny rad i Poststeder
                else
                {
                    var nyKunde = new KundeDB
                    {
                        bankId = bankId,
                        personNr = kunde.personNr,
                        passord = krypterPassord(lagPassord(), salt),
                        salt = salt,
                        fornavn = kunde.fornavn,
                        etternavn = kunde.etternavn,
                        adresse = kunde.adresse,
                        telefonNr = kunde.telefonNr,
                        postNr = kunde.postNr
                    };

                    var nyPoststed = new PoststedDB
                    {
                        postNr = kunde.postNr,
                        poststed = kunde.poststed
                    };
                    try
                    {
                        nyKunde.poststed = nyPoststed;
                        db.Kunder.Add(nyKunde);
                        db.SaveChanges();
                        opprettStandardkonto(kundeInfo);
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under registrering av kunde! - "
                            + feil.Message + " - " + feil.InnerException, false);
                        return false;
                    }

                }

                loggHendelse("Admin har opprettet en ny kunde med navn(" + kunde.fornavn + " " + kunde.etternavn + ")" +
                    " og personnummer(" + kunde.personNr + ")", true);
                return true;
            }
        }

        //Her genereres tilfeldig passord for en ny kunde som admin lager,passordet skal da sendes 
        //til kunden på mail eller sms.
        public String lagPassord()
        {
            string velgFra = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-";
            char[] bokstaver = new char[9];
            Random tilfeldig = new Random();

            for (int i = 0; i < 9; i++)
            {
                bokstaver[i] = velgFra[tilfeldig.Next(0, velgFra.Length)];
            }

            return new string(bokstaver);
        }

        //Admin skal kunne endre eksisterende kunde info om nødvendig.
        public Boolean endreKunde(String idnr, Kunde innKunde)
        {

            using (var db = new DBContext())
            {
                try
                {
                    KundeDB endreKunde = db.Kunder.Find(idnr);
                    endreKunde.personNr = innKunde.personNr;
                    endreKunde.fornavn = innKunde.fornavn;
                    endreKunde.etternavn = innKunde.etternavn;
                    endreKunde.adresse = innKunde.adresse;
                    endreKunde.telefonNr = innKunde.telefonNr;
                    if (endreKunde.postNr != innKunde.postNr)
                    {
                        // sjekker om postnr allerede finnes
                        bool finnes = db.Poststeder.Any(p => p.postNr == innKunde.postNr);
                        if (finnes)
                        {
                            // Om postedet ikke eksisterer så legges det inn her
                            var nyPoststed = new PoststedDB
                            {
                                postNr = innKunde.postNr,
                                poststed = innKunde.poststed
                            };

                        }
                        else
                        {   // Endrer postnr
                            endreKunde.postNr = innKunde.postNr;
                        }
                    };
                    db.SaveChanges();
                }
                catch (Exception feil)
                {
                    loggHendelse("Det oppstod en feil under endring av kunde(" + idnr + ") - "
                        + feil.Message + " - " + feil.InnerException, false);
                    return false;
                }

                loggHendelse("Admin har endret informasjon hos kunde(" + idnr + ")", true);
                return true;
            }
        }

        public Boolean slettKunde(string personNr)
        {
            Boolean OK = false;

            using (var db = new DBContext())
            {
                var ok = sjekkSaldo(personNr);

                try
                {
                    KundeDB slettKunde = db.Kunder.Find(personNr);

                    if (ok)
                    {
                        db.Kunder.Remove(slettKunde);
                        db.SaveChanges();
                        return true;
                    }
                    return false;
                }
                catch (Exception feil)
                {
                    return false;
                }
            }
        }

        //Under her er listet søkfunksjoner, skal kunne søke en kunde via tlf,personnr,navn og eventuelt liste opp alle kunder med et gitt postnr
        public Kunde finnKunde(string sok)
        {
            using (var db = new DBContext())
            {
                var funnetKunde = db.Kunder.Find(sok);

                if (funnetKunde == null)
                {
                    return null;
                }
                else
                {
                    var utKunde = new Kunde()
                    {
                        personNr = funnetKunde.personNr,
                        fornavn = funnetKunde.fornavn,
                        etternavn = funnetKunde.etternavn,
                        adresse = funnetKunde.adresse,
                        postNr = funnetKunde.postNr,
                        telefonNr = funnetKunde.telefonNr,
                        poststed = funnetKunde.poststed.poststed
                    };
                    return utKunde;
                }
            }
        }
       
        public Boolean sjekkSaldo(String personnr)
        {
            var ok = false;
            using (var db = new DBContext())
            {
                var sjekkSaldo = db.Kontoer.Where(k =>k.personNr == personnr);
                
                foreach (var i in sjekkSaldo)
                {
                    if(i.saldo == 0)
                    {
                        ok = true;
                    }
     
                }

            }
            return ok;
        }
    
        // ---------------------------------------------------------------------------------------
        // Kunde Metoder

        // Metode for kryptering av passord + salt
        public static String krypterPassord(String passord, byte[] salt)
        {
            String utPassord;
            byte[] innPassord, tmpPassord, utdata;

            // Lagrer passord i bytearray og oppretter krypteringsalgoritme
            innPassord = Encoding.ASCII.GetBytes(passord);
            var algoritme = SHA512.Create();

            // Legger til saltet på slutten av passordet
            tmpPassord = new byte[innPassord.Length + salt.Length];

            for (int i = 0; i < innPassord.Length; i++)
            {
                tmpPassord[i] = innPassord[i];
            }
            for (int i = 0; i < salt.Length; i++)
            {
                tmpPassord[innPassord.Length + i] = salt[i];
            }

            // Kyrpterer bytearrayet og gjør det om til en string før retur
            utdata = algoritme.ComputeHash(tmpPassord);
            utPassord = Encoding.ASCII.GetString(utdata);

            return utPassord;
        }

        // Genererer tilfeldig salt
        public static byte[] genererSalt()
        {
            // Fyller ut et byte array med tilfeldige bytes
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            // Genererer et tilfeldig tall mellom 16 og 64
            Random rnd = new Random();
            int num = rnd.Next(16,64);

            byte[] buffer = new byte[num];
            rng.GetBytes(buffer);

            return buffer;
        }
          
        // Registrering av kunde. Tar et Kunde objekt direkte dra Html.beginForm()
        public Boolean registrerKunde(Kunde kunde) 
        {
            // Oppretter Database connection
            using (var db = new DBContext())
            {
                int bid = db.Kunder.Count();
                bid += 1;
                String bankId = bid.ToString();
                string[] kundeInfo = { kunde.fornavn, kunde.etternavn, kunde.personNr };
                byte[] salt = genererSalt();

                // Sjekker om postnr og poststed allerede finnes
                bool finnes = db.Poststeder.Any(p => p.postNr == kunde.postNr);

                // Om postnr og poststed finnes så opprettes en ny kunde 
                // uten noe i Poststed klasse-attributett til kunden
                if (finnes)
                {
                    var nyKunde = new KundeDB
                    {
                        bankId = bankId,
                        personNr = kunde.personNr,
                        passord = krypterPassord(kunde.passord, salt),
                        salt = salt,
                        fornavn = kunde.fornavn,
                        etternavn = kunde.etternavn,
                        adresse = kunde.adresse,
                        telefonNr = kunde.telefonNr,
                        postNr = kunde.postNr
                    };

                    try
                    {
                        db.Kunder.Add(nyKunde);
                        db.SaveChanges();
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under registrering av kunde! - " 
                            + feil.Message +  " - " + feil.InnerException, false);
                        return false;
                    }

                }
                // Postnr og poststed finnes ikke, 
                // legger inne kunden og oppretter en ny rad i Poststeder
                else
                {
                    var nyKunde = new KundeDB
                    {
                        bankId = bankId,
                        personNr = kunde.personNr,
                        passord = krypterPassord(kunde.passord, salt),
                        salt = salt,
                        fornavn = kunde.fornavn,
                        etternavn = kunde.etternavn,
                        adresse = kunde.adresse,
                        telefonNr = kunde.telefonNr,
                        postNr = kunde.postNr
                    };

                    var nyPoststed = new PoststedDB
                    {
                        postNr = kunde.postNr,
                        poststed = kunde.poststed
                    };
                    try
                    {
                        nyKunde.poststed = nyPoststed;
                        db.Kunder.Add(nyKunde);
                        db.SaveChanges();
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under registrering av kunde! - " 
                            + feil.Message + " - " + feil.InnerException, false);
                        return false;
                    }

                }

                // Lagrer bankid til den nyregistrerte kunden, dersom den trengs ved førstegangs-innlogging
                HttpContext.Current.Session["bankid"] = bankId;

                // Oppretter standardkonto til kunden
                opprettStandardkonto(kundeInfo);

                // Kunden logges dersom det ikke var noen excpetions som ble fanget
                loggHendelse("Ny kunde har blitt registrert med navn(" + kunde.fornavn + " " + kunde.etternavn + ") og " 
                    + "personnummer(" + kunde.personNr + ")", true);

                return true;
            }
        }

        //Metode for registrering av ny konto
        public Boolean registrerNyKonto(Konto nykonto)
        {
            Boolean OK = false;

            using (var db = new DBContext())
            {
                var nyKonto = new KontoDB()
                {
                    kontoNr = nykonto.kontoNr,
                    saldo = nykonto.saldo,
                    kontoNavn = nykonto.kontoNavn, 
                    personNr = nykonto.personNr
                };
                try
                {
                    db.Kontoer.Add(nyKonto);
                    db.SaveChanges();
                    OK = true;
                }
                catch (Exception feil)
                {
                    loggHendelse("Det oppstod en feil under registrering av ny konto for kunde(" 
                        + nykonto.personNr + ") - " + feil.Message + " - " + feil.InnerException, false);
                    return false;
                }

                if (OK)
                {
                    // gjøre noe
                }
            }

            return true;
        }

        // Innloggingsmetode for kunder
        public Boolean kundeLogginn(Kunde kunde)
        {
            using (var db = new DBContext())
            {
                // Sjekker oppgitte personnr og bankid mot databasen om kunden finnes
                KundeDB fantKunde = db.Kunder.FirstOrDefault
                    (k => k.personNr == kunde.personNr && k.bankId == kunde.bankId);

                if (fantKunde != null)
                {
                    // Dersom kunden finnes så sjekker vi om oppgitte passord er korrekt
                    String passord = krypterPassord(kunde.passord, fantKunde.salt);
                    if (passord == fantKunde.passord)
                    {
                        return true;
                    }
                }

                return false;
            }

        }
        // Registrerer transaksjon
        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            String personnr = (String)HttpContext.Current.Session["personnr"];

            using (var db = new DBContext())
            {
                KontoDB funnetKonto = db.Kontoer.FirstOrDefault(k => k.kontoNavn == transaksjon.fraKonto);
                if (funnetKonto != null)
                {
                    transaksjon.status = "venter";
                    transaksjon.saldoInn = 0;

                    var nytransaksjon = new TransaksjonDB()
                    {
                        status = transaksjon.status,
                        saldoInn = transaksjon.saldoInn,
                        saldoUt = transaksjon.saldoUt,
                        dato = transaksjon.dato,
                        KID = transaksjon.KID,
                        fraKonto = transaksjon.fraKonto,
                        tilKonto = transaksjon.tilKonto,
                        melding = transaksjon.melding,
                        konto = funnetKonto
                    };
                    
                    try
                    {
                        db.Transaksjoner.Add(nytransaksjon);
                        db.SaveChanges();
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under registrering av transaksjon for kunde(" 
                            + funnetKonto.personNr + ") - " + feil.Message + " - " + feil.InnerException, false);
                        return null;
                    }

                    loggHendelse("Ny transaksjon utført for kunde(" + personnr + ") fra konto(" 
                        + nytransaksjon.fraKonto + ") med KID(" + nytransaksjon.KID + ")", true);
                    return transaksjon;
                }

                return null;
            }
        }

        // Henter alle kontoer som tilhører gitt personnr
        public List<String> hentKontoer(String personnr)
        {
            var kontoer = new List<String>();

            using (var db = new DBContext())
            {
                // henter alle kontoer
                var tmp = db.Kontoer.ToList();
                // Velger kun kontoene med gitte personnr
                foreach (var k in tmp)
                {
                    if (personnr == k.personNr)
                    {
                        kontoer.Add(k.kontoNavn);
                    }
                }
            }

            return kontoer.ToList();
        }

        // Meetode som lager tabellen for konto informasjon
        public String hentKontoInformasjon(String kontonavn, String personnr)
        {
            String kontoInformasjon =
                "<p><h3>Konto informasjon</h3></p>" +
                "<table>" +
                "<tr>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Kontonavn</th>" +
                "<th class='col-sm-4' style='background-color:lavender;'>Kontonummer</th>" +
                "<th class='col-sm-4' style='background-color:lavenderblush;'>Saldo</th>" +
                "</tr>";

            // Finner korrekt konto og kunde
            using (var db = new DBContext())
            {
                var info = db.Kontoer.Where(k => k.kontoNavn == kontonavn && k.personNr == personnr);
                foreach (var i in info)
                {
                    kontoInformasjon +=
                        "<tr>" +
                        "<td class='col-sm-4' style='background-color:lavenderblush;'>"+ i.kontoNavn+"</td>" +
                        "<td class='col-sm-4' style='background-color:lavender;'>"+i.kontoNr+"</td>" +
                        "<td class='col-sm-4' style='background-color:lavenderblush;'>"+i.saldo+"</td>" +
                        "</tr>";
                }
            }

            kontoInformasjon += "</table>";
            return kontoInformasjon;
        }

        // Metode som lager tabell for kontoutskrifter
        public String hentKontoUtskrift(String kontonavn, String personnr) 
        {
            String kontoUtskrift =                        
                "<p><h3>Konto utskrift</h3></p>" +
                "<table>" +
                "<tr>" +
                "<th class='col-sm-1' style='background-color:lavenderblush;'>Status</th>" +
                "<th class='col-sm-1' style='background-color:lavender;'>Dato</th>" +
                "<th class='col-sm-1' style='background-color:lavenderblush;'>KID</th>" +
                "<th class='col-sm-1' style='background-color:lavender;'>Saldo inn</th>" +
                "<th class='col-sm-1' style='background-color:lavenderblush;'>Saldo ut</th>" +
                "<th class='col-sm-1' style='background-color:lavender;'>Fra konto</th>" +
                "<th class='col-sm-1' style='background-color:lavenderblush;'>Til konto</th>" +
                "<th class='col-sm-1' style='background-color:lavender;'>Melding</th>" +
                "</tr>";

            // Finner riktig konto og kunde
            using (var db = new DBContext())
            {
                String status, dato, kid, saldoinn, saldout, frakonto, tilkonto, melding;
                for (int i =1;i<= db.Transaksjoner.Count(); i++)
                {
                    var td= db.Transaksjoner.SingleOrDefault(x => x.Id == i);
                    status = td.status;
                    dato = td.dato;
                    kid = td.KID;
                    saldoinn = td.saldoInn+"";
                    saldout = td.saldoUt + "";
                    frakonto = td.fraKonto;
                    tilkonto = td.tilKonto;
                    melding = td.melding;

                    if (frakonto.Equals(kontonavn))
                    {
                        kontoUtskrift +=
                      "<tr>" +
                      "<td class='col-sm-1' style='background-color:lavenderblush;'>" + status + "</td>" +
                      "<td class='col-sm-1' style='background-color:lavender;'>" + dato + "</td>" +
                      "<td class='col-sm-1' style='background-color:lavenderblush;'>" + kid + "</td>" +
                      "<td class='col-sm-1' style='background-color:lavender;'>" + "0" + "</td>" + 
                      "<td class='col-sm-1' style='background-color:lavenderblush;'>" + saldout+ "</td>" +
                      "<td class='col-sm-1' style='background-color:lavender;'>" + frakonto + "</td>" +
                      "<td class='col-sm-1' style='background-color:lavenderblush;'>" + tilkonto + "</td>" +
                      "<td class='col-sm-1' style='background-color:lavender;'>" + melding + "</td>" +
                      "</tr>";
                    } else if (tilkonto.Equals(kontonavn)) {
                        kontoUtskrift +=
                       "<tr>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + status + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + dato + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + kid + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + saldout + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + "0" + "</td>" + 
                       "<td class='col-sm-1' style='background-color:lavender;'>" + frakonto + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + tilkonto + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + melding + "</td>" +
                       "</tr>";
                    }
                   
                }
            }

            kontoUtskrift += "</table>";
            return kontoUtskrift;
        }

        // Sjekker ved hver oppstart om dummy data finnes
        public void startsjekk()
        {
            var db = new DBContext();
            try
            {
                var enDbKunde = db.Kunder.First();
            }
            catch (Exception feil)
            {
                dummyData();
            }
        }


        // Oppretter dummy data dersom databasen er tom
        public static void dummyData()
        {
            string[] fornavn = new string[] { "Per", "Ola", "Maria", "Marius", "Helen", "Brage", "Najmi", "Eirik", "Martin" };
            string[] etternavn = new string[] { "Bakke", "Hansen", "Dilora", "Kalle", "Desta", "Petter", "Suda", "Solo", "Haugen" };
            string[] poststed = new string[] { "Oslo", "Bergen", "Stavanger", "Kristia", "Haugesund", "Hammer", "Oslo", "Langesund", "Skien" };
            string[] adresse = new string[] { "Helba 2", "Femti 21", "Hokk 34", "Turn 12", "Kort 22", "Malibu 2", "Half Life 3", "Acestreet 13", "Gangveien 9" };
            byte[] kundeSalt, adminSalt;

            int pernr = 118921160, tlf = 12345678, konNr = 12345, postNr = 6789, bankid = 0, adminid = 0;
            AdminDB a;
            KundeDB k;
            PoststedDB p;
            KontoDB s;

            using (var db = new DBContext())
            {
                for (var i = 0; i < fornavn.Length; i++)
                {
                    pernr += i;
                    tlf += 1;
                    konNr += 1;
                    postNr += 1;
                    bankid += 1;
                    kundeSalt = genererSalt();
                    
                    // Lager 7 Kundekontoer
                    if (i < fornavn.Length - 2)
                    {
                        k = new KundeDB();
                        p = new PoststedDB();
                        s = new KontoDB();

                        k.bankId = bankid.ToString();
                        p.poststed = poststed[i];
                        k.personNr = pernr.ToString();
                        k.passord = krypterPassord("passord", kundeSalt);
                        k.salt = kundeSalt;
                        k.fornavn = fornavn[i];
                        k.etternavn = etternavn[i];
                        k.adresse = adresse[i];
                        k.telefonNr = tlf.ToString();
                        k.postNr = p.postNr = postNr.ToString();
                        k.poststed = p;

                        s.kontoNr = konNr.ToString();
                        s.saldo = 500;
                        s.kontoNavn = ""+konNr;
                        s.personNr = k.personNr;

                        try
                        {
                            db.Kunder.Add(k);
                            db.Kontoer.Add(s);
                            db.SaveChanges();
                        }
                        catch (Exception feil)
                        {
                            loggHendelse("Det oppstod en feil under opprettelse av dummydata! - " 
                                + feil.Message, false);
                            return;
                        }

                        // Legger til esktra kontoer en gang 
                        if (i == fornavn.Length - 3)
                        {
                            konNr += i;
                            //2 ekstra kontoer for personNR 1 og 1 ekstra konto for person nr 2!
                            KontoDB e = new KontoDB();
                            e.kontoNr = konNr.ToString();
                            e.saldo = 50;
                            e.kontoNavn = "" + konNr;
                            e.personNr = "118921160";

                            konNr += i;
                            KontoDB f = new KontoDB();
                            f.kontoNr = konNr.ToString();
                            f.saldo = 400;
                            f.kontoNavn = "" + konNr;
                            f.personNr = "118921160";

                            konNr += i;
                            KontoDB g = new KontoDB();
                            g.kontoNr = konNr.ToString();
                            g.saldo = 50;
                            g.kontoNavn = "" + konNr;
                            g.personNr = "118921161";

                            try
                            {
                                db.Kontoer.Add(e);
                                db.Kontoer.Add(f);
                                db.Kontoer.Add(g);
                                db.SaveChanges();
                            }
                            catch (Exception feil)
                            {
                                loggHendelse("Det oppstod en feil under opprettelse av dummydata! - " 
                                    + feil.Message, false);
                                return;
                            }

                        }
                    }

                    else
                    {
                        adminid += 1;
                        adminSalt = genererSalt();

                        a = new AdminDB();
                        p = new PoststedDB();

                        a.adminId = adminid.ToString();
                        a.passord = krypterPassord("ghettoadmin", adminSalt);
                        a.salt = adminSalt;
                        a.fornavn = fornavn[i];
                        a.etternavn = etternavn[i];
                        a.adresse = adresse[i];
                        a.telefonNr = tlf.ToString();

                        p.poststed = poststed[i];
                        a.postNr = p.postNr = postNr.ToString();
                        a.poststed = p;
                        try
                        {
                            db.Admins.Add(a);
                            db.SaveChanges();
                        }
                        catch (Exception feil)
                        {
                            loggHendelse("Det oppstod en feil under opprettelse av dummydata! - " 
                                + feil.Message, false);
                            return;
                        }
                    }

                } // forloop slutt

            } // DB slutt

            loggHendelse("Dummydata opprettet", true);
        }

        // Oppretter en standard konto for hver ny kunde registrring
        public void opprettStandardkonto(string[] nyKundeInfo)
        {
            int n;
            Boolean OK = false;

            using (var db = new DBContext())
            {
                n = db.Kunder.Count();
                string kontonr = 3211 + "" + n;

                var nykonto = new KontoDB()
                {
                    kontoNr = kontonr,
                    saldo = 0,
                    kontoNavn = ""+ kontonr,
                    personNr = nyKundeInfo[2],
                };

                try
                {
                    db.Kontoer.Add(nykonto);
                    db.SaveChanges();
                    OK = true;
                }
                catch (Exception feil)
                {
                    loggHendelse("Det oppstod en feil under opprettelse av standardkonto for kunde(" + nyKundeInfo[2] + ") - " 
                        +  feil.Message + " - " + feil.InnerException, false);
                }

                if (OK)
                {
                    loggHendelse("Ny Standardkonto(" + nykonto.kontoNr +
                        ") har blitt opprettet for kunde med personnummer( " + nykonto.personNr + ")", true);
                }

            }
        }

        // Metode for å oppdatere kontobalanser etter transaksjoner
        public void oppdaterKontoer(String fraKonto, String tilKonto, String belop)
        {
            Boolean OK = false;
           // string personnr = (String)HttpContext.Current.Session["personnr"]; 

            using (var db = new DBContext())
            {
                    var fraKontoFunnet = db.Kontoer.Find(fraKonto);
                    var tilKontoFunnet = db.Kontoer.Find(tilKonto);

                    int fraKontoSinBalanse = fraKontoFunnet.saldo;
                    int tilKontoSinBalanse = tilKontoFunnet.saldo;

                    int kontoSomSkalBetalesFraSinNyeBalanse = fraKontoSinBalanse - Int32.Parse(belop);
                    int kontoSomSkalBetalesTilSinNyeBalanse = tilKontoSinBalanse + Int32.Parse(belop);

                    fraKontoFunnet.saldo = kontoSomSkalBetalesFraSinNyeBalanse;
                    tilKontoFunnet.saldo = kontoSomSkalBetalesTilSinNyeBalanse;

                    try
                    {
                        db.SaveChanges();
                        OK = true;
                    }
                    catch (Exception feil)
                    {
                        loggHendelse("Det oppstod en feil under overføring av saldo fra konto(" + fraKonto +
                            ") til konto(" + tilKonto + ") - " + feil.Message + " - " + feil.InnerException, false);
                    }

                    if (OK)
                    { 
                      /*  loggHendelse("kunde(" + personnr + ") har overført " + belop + 
                            "kr fra konto(" + fraKontoFunnet.kontoNr + ") til konto(" + tilKontoFunnet.kontoNr + ")", true);                  
                        loggHendelse("kunde(" + tilKontoFunnet.personNr + ") har motatt " + belop + 
                            "kr på konto( " + tilKontoFunnet.kontoNr + "), fra konto(" + fraKontoFunnet.kontoNr + ")", true);
                            */
                    }
            }
        }

        // Metode som henter veien til C:\Users\DITTBRUKERNAVN\AppData\Temp
        public static String hentTempPath()
        {
            String path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }

        // Metode logging av hendelse til fil
        public static void loggHendelse(String melding, Boolean type)
        {
            
            System.IO.StreamWriter writer;

            // Åpner en stream writer med filnavn "DBendringer" dersom type parameter er true. 
            // Dette logger alle endringer gjort mot Database.
            if (type == true)
            {
                writer = System.IO.File.AppendText(hentTempPath() + "Nettbanken - Databaseendringer.txt");
            }
            // Åpner en stream writer med filnavn "DBfeil" dersom type parameter er  false.
            // Dette logger alle feilsituasjoner. 
            else
            {
                writer = System.IO.File.AppendText(hentTempPath() + "Nettbanken - Feilsituasjoner.txt");
            }

            // Logger hendelsen til filen
            try
            {
                String loggLinje = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, melding);
                writer.WriteLine(loggLinje);
            }
            finally
            {
                writer.Close();
            }
        }

        //sjekk transaksjonen og utfor, og opdater status.
        public void startSjekkTransaksjonStatus()
        {
            System.Diagnostics.Debug.WriteLine("SjekkingTRANSAKSJON......../////////////CHECK!//////////");
            using (var db = new DBContext())
            {
                int antallTransaksjoner = db.Transaksjoner.Count();
                int antallSjekketRad = 1;

                while (antallSjekketRad <= antallTransaksjoner)  
                {
                    var transaksjonData = db.Transaksjoner.SingleOrDefault(x => x.Id == antallSjekketRad);//hent en rad med id = i fra transaksjon

                    String datoIdagD = DateTime.Today.ToString("dd");
                    String datoIdagM = DateTime.Today.ToString("MM");
                    String datoIdagA = DateTime.Today.ToString("yyyy");

                    String datoIdag = datoIdagD + "/" + datoIdagM + "/" + datoIdagA; 
                    String transaksjonsDato = transaksjonData.dato;
                    String transaksjonStatus = transaksjonData.status; 

                    Boolean oppdateresIdag = (datoIdag.Equals(transaksjonsDato)) ? true : false;

                    String tD, tM, tA;
                    tD = transaksjonsDato.ElementAt(0) +""+ transaksjonsDato.ElementAt(1)+"";
                    tM = transaksjonsDato.ElementAt(3) + "" + transaksjonsDato.ElementAt(4) + "";
                    tA = transaksjonsDato.ElementAt(6) + "" + transaksjonsDato.ElementAt(7) + ""+ transaksjonsDato.ElementAt(8) + "" + transaksjonsDato.ElementAt(9) + "";

                    int transaksjonsDatoSinDATO = Int32.Parse(tD); 
                    int transaksjonsDatoSinMANE = Int32.Parse(tM);
                    int transaksjonsDatoSinAAR = Int32.Parse(tA);

                    //error kunne lurt seg inn her utsagnet.. sjekk gjerne igjen.
                    Boolean aarHarPasert = transaksjonsDatoSinAAR < Int32.Parse(datoIdagA);
                    Boolean maneHarPasert = transaksjonsDatoSinAAR == Int32.Parse(datoIdagA) && transaksjonsDatoSinMANE < Int32.Parse(datoIdagM);
                    Boolean datoHarPasert = transaksjonsDatoSinAAR == Int32.Parse(datoIdagA) && transaksjonsDatoSinMANE == Int32.Parse(datoIdagM) && transaksjonsDatoSinDATO < Int32.Parse(datoIdagD);

                    Boolean betalingsDatoHarPasert = (aarHarPasert || maneHarPasert || datoHarPasert) ? true : false;

                    if ((oppdateresIdag && transaksjonData.status.Equals("venter") && antallTransaksjoner != 0) || (transaksjonData.status.Equals("venter") && betalingsDatoHarPasert && antallTransaksjoner != 0))
                    {
                        if (db.Kontoer.Find(transaksjonData.tilKonto)!= null) { 
                        oppdaterKontoer(transaksjonData.fraKonto, transaksjonData.tilKonto, transaksjonData.saldoUt + "");
                        transaksjonData.status = "betalt";
                        System.Diagnostics.Debug.WriteLine("\nTRANSAKSJON_BETALING_UTFORT!/////////////UTFORT////////////////////\n");
                        }else
                        {
                            transaksjonData.status = "FEILET";
                            System.Diagnostics.Debug.WriteLine("\nTRANSAKSJON_BETALING_FEILET!/////////FEILET////////////////////////\n");
                        }
                    }
                    antallSjekketRad += 1;
                }//end forloop
                try
                {
                    db.SaveChanges();
                }
                catch (Exception feil)
                {
                    // System.Diagnostics.Debug.WriteLine("database feilet aa lagre! " + feil);
                }
            }
        }

    }
}