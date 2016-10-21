﻿using Nettbanken.Controllers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Nettbanken.Models
{
    public class DBMetoder
    {
       private static int bankId = 0;

        // Metode for kryptering av passord
        private static String krypterPassord(String passord)
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
        public static String registrerKunde(Kunde kunde,bool erIkkeDummy) 
        {
           
            String OK = "";

            // Oppretter Database connection
            using (var db = new DbModell())
            {
                int bid = db.Kunder.Count(); 
                bid += 1;
                String bankId = bid + "";
                // Sjekker om postnr og poststed allerede finnes
                bool finnes = db.Poststeder.Any(p => p.postNr == kunde.poststed.postNr);
                // Om postnr og poststed finnes så opprettes en ny kunde 
                // uten noe i Poststed klasse-attributett til kunden
                if (finnes)
                {
                    var nyKunde = new Kunde
                    {
                        //bankId = "1337",
                        bankId = bankId,
                        personNr = kunde.personNr,
                        passord = krypterPassord(kunde.passord),
                        fornavn = kunde.fornavn,
                        etternavn = kunde.etternavn,
                        adresse = kunde.adresse,
                        telefonNr = kunde.telefonNr,
                        postNr = kunde.poststed.postNr
                    };

                    try
                    {
                        db.Kunder.Add(nyKunde);
                        db.SaveChanges();
                       
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
                    // kunde.bankId = "1337";
                    kunde.bankId = bankId; 
                    kunde.passord = krypterPassord(kunde.passord);
                    try
                    {
                       
                        db.Kunder.Add(kunde);
                        db.SaveChanges();
                        if (erIkkeDummy)
                        {
                            string[] kundeInfo = { kunde.fornavn, kunde.etternavn, kunde.personNr };
                            KundeController.opprettNyKontoVedNyKundeRegistrering(kundeInfo);
                        }
                    }
                    catch (DbEntityValidationException deve)
                    {
                        OK = "Det oppstod en feil i registrering av kunden! Feil: " + deve.Message;
                        //skriv ut feilen spesifikt
                        foreach (var validationErrors in deve.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                    }
                }
             
            }
            

            return OK;
        }

        //Konto registrering: opprette konto for kunder samtidig som di registreres!
        public static bool registrerNyKonto(Konto nyk)
        {
            using (var db = new DbModell())
            {
                var nyKonto = new Konto()
                {
                    kontoNr = nyk.kontoNr,
                    saldo = nyk.saldo,
                    kontoNavn = nyk.kontoNavn, 
                    personNr = nyk.personNr

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

        // Innloggingsmetode for kunder
        public static Boolean kundeLogginn(Kunde kunde)
        {
            using (var db = new DbModell())
            {
                // krypterer det gitte passordet 
                // og sjekker oppgitte personnr og passord mot database
                String passord = krypterPassord(kunde.passord);
                Kunde fantKunde = db.Kunder.FirstOrDefault
                    (k => k.personNr == kunde.personNr && k.passord == passord && k.bankId == kunde.bankId);

                if (fantKunde != null)
                {
                    return true;
                }

                return false;
            }

        }

        public static Transaksjon registrerTransaksjon(String personnr, Transaksjon t)
        {
            using (var db = new DbModell())
            {
                Konto funnetKonto = db.Kontoer.FirstOrDefault(k => k.kontoNavn == t.fraKonto);
                if (funnetKonto != null)
                {
                    var transaksjon = new Transaksjon()
                    {
                        status = "Midlertidig Status",
                        saldoInn = 0,
                        saldoUt = t.saldoUt,
                        dato = t.dato,
                        KID = t.KID,
                        fraKonto = funnetKonto.kontoNr,
                        tilKonto = t.tilKonto,
                        melding = t.melding,
                        konto = funnetKonto
                    };
                    try
                    {
                        db.Transaksjoner.Add(transaksjon);
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
        public static List<String> hentKontoer(String personnr)
        {
            var kontoer = new List<String>();

            using (var db = new Models.DbModell())
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
        public static String hentKontoInformasjon(String kontonavn, String personnr)
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
            using (var db = new DbModell())
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
        public static String hentKontoUtskrift(String kontonavn, String personnr) 
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
            using (var db = new DbModell())
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

        /* @@@@@@@@@ CATCH METODE SOM FANGER OPP EN UNIK FEIL, IKKE SLETT@@@@@@@@@
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }
             }
        */
    }
}