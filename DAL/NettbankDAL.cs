﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nettbanken.Models;

namespace Nettbanken.DAL
{
    // Klasse for alle metoder som interagerer med databasen, aka DAL
    public class NettbankDAL
    {
       private static int bankId = 0;

        // Metode for kryptering av passord
        public static String krypterPassord(String passord)
        {
            String innPassord, utPassord;
            byte[] inndata, utdata;

            // Lagrer passord og oppretter krypteringsalgoritme
            innPassord = passord;
            var algoritme = System.Security.Cryptography.SHA512.Create();

            // gjør string om til byte array og krypterer det
            inndata = System.Text.Encoding.ASCII.GetBytes(innPassord);
            utdata = algoritme.ComputeHash(inndata);

            utPassord = System.Text.Encoding.ASCII.GetString(utdata);

            return utPassord;
        }
        
        
        // Registrering av kunde. Tar et Kunde objekt direkte dra Html.beginForm()
        public String registrerKunde(Kunde kunde) 
        {         
            String OK = "";

            // Oppretter Database connection
            using (var db = new DBContext())
            {
                int bid = db.Kunder.Count(); 
                bid += 1;
                String bankId = bid.ToString();
                string[] kundeInfo = { kunde.fornavn, kunde.etternavn, kunde.personNr };
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
                        passord = krypterPassord(kunde.passord),
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
                        OK = "Det oppstod en feil i registrering av kunden! Feil: " + feil.Message;
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
                        passord = krypterPassord(kunde.passord),
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
                        OK = "Det oppstod en feil i registrering av kunden! Feil: " + feil.Message + feil.InnerException;
                    }

                }
             
            }
            

            return OK;
        }

        //Konto registrering: opprette konto for kunder samtidig som di registreres!
        public bool registrerNyKonto(Konto nykonto)
        {
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
                }
                catch (Exception e)
                {
                    //db failure
                    string ex = e.ToString();
                    return false;
                }
            }
            return true;
        }

        // Innloggingsmetode for admins
        public Boolean adminLogginn(Admin admin)
        {
            using (var db = new DBContext())
            {
                // krypterer det gitte passordet 
                // og sjekker oppgitte personnr og passord mot database
                String passord = krypterPassord(admin.passord);
                AdminDB fantAdmin = db.Admins.FirstOrDefault
                    (a => a.adminId == admin.adminId && a.passord == passord);

                if (fantAdmin != null)
                {
                    return true;
                }

                return false;
            }
        }

        // Innloggingsmetode for kunder
        public Boolean kundeLogginn(Kunde kunde)
        {
            using (var db = new DBContext())
            {
                // krypterer det gitte passordet 
                // og sjekker oppgitte personnr og passord mot database
                String passord = krypterPassord(kunde.passord);
                KundeDB fantKunde = db.Kunder.FirstOrDefault
                    (k => k.personNr == kunde.personNr && k.passord == passord && k.bankId == kunde.bankId);

                if (fantKunde != null)
                {
                    return true;
                }

                return false;
            }

        }
        // Registrerer transaksjon
        public Transaksjon registrerTransaksjon(Transaksjon transaksjon)
        {
            using (var db = new DBContext())
            {
                KontoDB funnetKonto = db.Kontoer.FirstOrDefault(k => k.kontoNavn == transaksjon.fraKonto);
                if (funnetKonto != null)
                {
                    transaksjon.status = "Midlertidig Status";
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
                        return transaksjon;
                    }
                    catch (Exception feil)
                    {

                    }
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
                var transaksjoner = db.Transaksjoner.Where(t => t.konto.kontoNavn == kontonavn && t.konto.personNr == personnr);
                foreach (var t in transaksjoner)
                {
                    kontoUtskrift +=
                       "<tr>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>"+ t.status +"</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>"+t.dato+"</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + t.KID + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + t.saldoInn + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + t.saldoUt + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + t.fraKonto + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavenderblush;'>" + t.tilKonto + "</td>" +
                       "<td class='col-sm-1' style='background-color:lavender;'>" + t.melding + "</td>" +
                       "</tr>";
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
            catch (Exception e)
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
            string[] adresse = new string[] { "Helba 2", "Femti 21", "Hokk 34", "Turn 12", "Kort 22", "Malibu 2", "Halv Life 3", "Acestreet 13", "Gangveien 9" };
            string kundePassord = krypterPassord("passord");
            string adminPassord = krypterPassord("ghettoadmin");

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

                    // Lager 7 Kundekontoer
                    if (i < fornavn.Length - 2)
                    {
                        k = new KundeDB();
                        p = new PoststedDB();
                        s = new KontoDB();

                        k.bankId = bankid.ToString();
                        p.poststed = poststed[i];
                        k.personNr = pernr.ToString();
                        k.passord = kundePassord;
                        k.fornavn = fornavn[i];
                        k.etternavn = etternavn[i];
                        k.adresse = adresse[i];
                        k.telefonNr = tlf.ToString();
                        k.postNr = p.postNr = postNr.ToString();
                        k.poststed = p;

                        s.kontoNr = konNr.ToString();
                        s.saldo = 500;
                        s.kontoNavn = k.fornavn + " " + k.etternavn + ": " + konNr;
                        s.personNr = k.personNr;

                        try
                        {
                            db.Kunder.Add(k);
                            db.Kontoer.Add(s);
                            db.SaveChanges();
                        }
                        catch (Exception e) { }

                        // Legger til esktra kontoer en gang 
                        if (i == fornavn.Length - 3)
                        {
                            konNr += i;
                            //2 ekstra kontoer for personNR 1 og 1 ekstra konto for person nr 2!
                            KontoDB e = new KontoDB();
                            e.kontoNr = konNr.ToString();
                            e.saldo = 50;
                            e.kontoNavn = "Per" + " " + "Bakke" + ": " + konNr;
                            e.personNr = "118921160";

                            konNr += i;
                            KontoDB f = new KontoDB();
                            f.kontoNr = konNr.ToString();
                            f.saldo = 400;
                            f.kontoNavn = "Per" + " " + "Bakke" + ": " + konNr;
                            f.personNr = "118921160";

                            konNr += i;
                            KontoDB g = new KontoDB();
                            g.kontoNr = konNr.ToString();
                            g.saldo = 50;
                            g.kontoNavn = "Ola" + " " + "Hansen" + ": " + konNr;
                            g.personNr = "118921161";

                            try
                            {
                                db.Kontoer.Add(e);
                                db.Kontoer.Add(f);
                                db.Kontoer.Add(g);
                                db.SaveChanges();
                            }
                            catch (Exception x) { }

                        }
                    }

                    else
                    {
                        adminid += 1;

                        a = new AdminDB();
                        p = new PoststedDB();

                        a.adminId = adminid.ToString();
                        a.passord = adminPassord;
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
                        catch (Exception feil) { }
                    }

                } // forloop slutt
            } // DB slutt

        }

        // Oppretter en standard konto for hver ny kunde registrring
        public void opprettStandardkonto(string[] nyKundeInfo)
        {
            int n;
            using (var db = new DBContext())
            {
                n = db.Kunder.Count();
                string kontonr = 3211 + "" + n;

                var nykonto = new KontoDB()
                {
                    kontoNr = kontonr,
                    saldo = 50,
                    kontoNavn = nyKundeInfo[0] + " " + nyKundeInfo[1] + ": " + kontonr,
                    personNr = nyKundeInfo[2],
                };

                try
                {
                    db.Kontoer.Add(nykonto);
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                }
            }
        }


    }
}