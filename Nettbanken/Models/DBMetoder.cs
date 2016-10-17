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