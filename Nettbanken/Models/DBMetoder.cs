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
        public static String registrerKunde(Kunde kunde)
        {
            String OK = "";
           
            // Oppretter Database connection
            using (var db = new DbModell())
            {
                // Sjekker om postnr og poststed allerede finnes
                bool finnes = db.Poststeder.Any(p => p.postNr == kunde.poststed.postNr);
                // Om postnr og poststed finnes så opprettes en ny kunde 
                // uten noe i Poststed klasse-attributett til kunden
                if (finnes)
                {
                    var nyKunde = new Kunde
                    {
                        bankId = "1337",
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
                    kunde.bankId = "1337";
                    kunde.passord = krypterPassord(kunde.passord);
                    try
                    {
                        db.Kunder.Add(kunde);
                        db.SaveChanges();
                    }
                    catch (Exception feil)
                    {
                        OK = "Det oppstod en feil i registrering av kunden! Feil: " + feil.Message;
                    }
                }
             
            }

            return OK;
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

        // Henter alle kontoer som tilhører gitt personnr
        public static List<String> hentKontoer(String personnr)
        {
            var kontoer = new List<String>();

            using (var db = new Models.DbModell())
            {
                // henter alle kontoer
                var tmp = db.Kontoer.ToList();
                // Velger kun kontoene med gitte personnr
                //kontoer.Add("---Velg konto---");
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

        public static String hentTransaksjoner(String kontonavn, String personnr)
        {
            String transkasjonerListe =                        
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

            using (var db = new DbModell())
            {
                var transaksjoner = db.Transaksjoner.Where(t => t.konto.kontoNavn == kontonavn && t.konto.personNr == personnr);
                foreach (var t in transaksjoner)
                {
                    transkasjonerListe +=
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

            transkasjonerListe += "</table>";
            return transkasjonerListe;
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