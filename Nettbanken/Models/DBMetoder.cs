using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nettbanken.Models
{
    public class DBMetoder
    {
        public static String skrivInnKunde(String[] a)
        {

            using (var db = new DbModell())
            {
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

                kunde.poststed = poststed;

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
    }
}