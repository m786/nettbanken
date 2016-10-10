using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbanken.Models
{
    public class DBMetoder
    {

        // Innloggingsmetode for kunder
        public static Kunde kundeLogginn(String[] a)
        {
            string bankid = a[0];
            string personnr = a[1];
            string passord = a[2];
            Kunde funnetKunde;

            using (var db = new DbModell())
            {
                funnetKunde = (from k in db.Kunder
                                     where k.bankId == bankid && k.personNr == personnr
                                           && k.passord == passord
                                     select k).Single();
            }

            return funnetKunde;
        }
        
        
        /*
        // Metode for å sette inn registreringsinfo til kundetabell
        public static String skrivInnKunde(String[] a)
        {
            // Åpner ny database connection
            using (var db = new DbModell())
            {
                // Ny Kunde objekt og setter inn data i kundetabell
                var kunde = new Kunde
                {
                    bankId = a[0],
                    personNr = a[1],
                    passord = a[2],
                    fornavn = a[3],
                    etternavn = a[4],
                    adresse = a[5],
                    telefonNr = a[6],
                    postNr = a[7]
                };

                var poststed = new Poststed
                {
                    postNr = a[7],
                    poststed = a[8]
                };
                // knytter kunde sin poststed med poststed tabellen
                kunde.poststed = poststed;

                // Prøver om innsetting er OK
                try
                {
                    db.Kunder.Add(kunde);
                    db.SaveChanges();
                    return "Innsetting av Kunde OK";
                }
                catch (Exception feil)
                {
                    return "Innsetting feil - " + feil.Message + " - " + feil.InnerException;
                }

            }
        }

        // Metode for å sette inn registreringsinfo til admintabell
        public static String skrivInnAdmin(String[] a)
        {
            // Åpner ny database connection
            using (var db = new DbModell())
            {
                // Ny admin objekt og setter inn data inn i admintabell
                var admin = new Admin
                {
                    adminId = a[0],
                    passord = a[1],
                    fornavn = a[2],
                    etternavn = a[3],
                    adresse = a[4],
                    telefonNr = a[5],
                    postNr = a[6]
                };

                var poststed = new Poststed
                {
                    postNr = a[6],
                    poststed = a[7]
                };
                // Knytter admin sin poststed med poststed tabellen
                admin.poststed = poststed;

                // Prøver om innsetting er OK
                try
                {
                    db.Admins.Add(admin);
                    db.SaveChanges();
                    return "Innsetting av Admin OK";
                }
                catch (Exception feil)
                {
                    return "Innsetting feil - " + feil.Message + " - " + feil.InnerException;
                }

            }
        }

        // Metode for å sette inn registreringsinfo til transaksjonstabell
        public static String skrivInnTransaksjon(String[] a)
        {
            // Åpner ny database connection
            using (var db = new DbModell())
            {
                // Ny Transaksjonsobjekt og setter data inn i tabellen
                var transaksjon = new Transaksjon
                {

                    status = a[0],
                    saldoInn = Int32.Parse(a[1]),
                    saldout = Int32.Parse(a[2]),
                    dato = a[3],
                    KID = a[4],
                    fraKonto = a[5],
                    tilKonto = a[6],
                    melding = a[7]
                };

                // Prøver om innsetting er OK
                try
                {
                    db.Transaksjoner.Add(transaksjon);
                    db.SaveChanges();
                    return "Innsetting av Transaksjon OK";
                }
                catch (Exception feil)
                {
                    return "Innsetting feil - " + feil.Message + " - " + feil.InnerException;
                }

            }
        }
        */
    }
}